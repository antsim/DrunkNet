using DrunkNet.API.Models;
using DrunkNet.API.Request;
using DrunkNet.API.Response;
using LiteDB;

namespace DrunkNet.API.Services;

public class DrinkService : IDrinkService
{
    private const int ResetDurationHours = 24;
    
    private readonly LiteDatabase _db;
    
    public DrinkService(ILiteDbContext liteDbContext)
    {
        _db = liteDbContext.Database;
    }
    
    public BloodAlcoholLevel AddDrink(DrinkRequest request, int userId)
    {
        // All DateTime values are converted to UTC on storage and converted back to local time on retrieval
        var currentTime = DateTime.Now;
        var userCol = _db.GetCollection<User>("users");
        var user = userCol.FindOne(u => u.Id == userId);

        var drinkCol = _db.GetCollection<Consumption>("drinks");
        var allUserDrinks = drinkCol.Find(d => d.User.Id == user.Id).ToList();

        if (allUserDrinks.Any())
        {
            var latestDrinkTime = allUserDrinks.Max(d => d.TimeConsumed);
            
            // If the last drink is over reset time limit
            // Delete all history, no one needs to remember how fun it was
            if (currentTime.Subtract(latestDrinkTime).TotalHours > ResetDurationHours)
            {
                drinkCol.DeleteMany(d => d.User == user);
            }
        }

        // Parse raw data
        // Data should be: <amount in dl> <alcohol percentage>
        // e.g. 4,4 6,8
        // = meaning 4,4 decilitres of 6,8% (beer)
        request.RawData = request.RawData.Replace('.', ',');
        var dataParts = request.RawData.Split(' ');

        if (!decimal.TryParse(dataParts[0], out var amount))
        {
            // Parse failed, so let's throw error
            throw new ArgumentException(
                $"Could not parse amount data '{dataParts[0]}' (full data: '{request.RawData}'");
        }
        
        if (!decimal.TryParse(dataParts[1], out var alcohol))
        {
            // Parse failed, so let's throw error
            throw new ArgumentException(
                $"Could not parse alcohol data '{dataParts[1]}' (full data: '{request.RawData}'");
        }
        
        var newDrink = new Consumption
        {
            User = user,
            TimeConsumed = currentTime,
            AlcoholContent = alcohol,
            DrinkAmount = amount,
            AlcoholMassInGrams = amount * 100 * (alcohol / 100) * 0.789M
        };
        
        drinkCol.Insert(newDrink);
        allUserDrinks.Add(newDrink);

        return GetBacInternal(userId, currentTime, allUserDrinks);
    }

    public BloodAlcoholLevel GetBac(int userId, List<Consumption> drinks)
    {
        return GetBacInternal(userId, DateTime.Now, drinks);
    }

    public List<BloodAlcoholLevel> GetBacHistory(int userId)
    {
        var userCol = _db.GetCollection<User>("users");
        var user = userCol.FindOne(u => u.Id == userId);

        var drinkCol = _db.GetCollection<Consumption>("drinks");
        var allUserDrinks = drinkCol.Find(d => d.User.Id == user.Id).ToList();

        if (!allUserDrinks.Any())
        {
            return new List<BloodAlcoholLevel>();
        }
        
        var firstDrinkAt = allUserDrinks.Min(d => d.TimeConsumed);
        var lastDrinkAt = allUserDrinks.Max(d => d.TimeConsumed);

        // We don't want to return data from previous times
        if (DateTime.Now.Subtract(lastDrinkAt).TotalHours > ResetDurationHours)
        {
            drinkCol.DeleteMany(d => d.User == user);
            return new List<BloodAlcoholLevel>();
        }
        
        var runningTime = firstDrinkAt;
        var getHistoryUntil = DateTime.Now;

        var bacHistory = new List<BloodAlcoholLevel>();

        while (runningTime < getHistoryUntil)
        {
            var bac = GetBacInternal(userId, runningTime, allUserDrinks);
            bacHistory.Add(bac);

            // if we have reached 0 BAC, we don't want to calculate the old drinks anymore
            if (bac.Bac == 0)
            {
                var time = runningTime;
                allUserDrinks.RemoveAll(d => d.TimeConsumed < time && d.User.Id == userId);
                drinkCol.DeleteMany(d => d.TimeConsumed < time && d.User.Id == userId);
            }
            
            if (bac.Bac == 0 && runningTime >= lastDrinkAt)
            {
                break;
            }
            
            runningTime = runningTime.AddMinutes(15);
        }

        // Get one more since the difference between the last loop and current time is less than 15 minutes
        bacHistory.Add(GetBacInternal(userId, getHistoryUntil, allUserDrinks));    
        
        return bacHistory;
    }

    public List<TopBacUser> GetTopList()
    {
        var userCol = _db.GetCollection<User>("users");
        var drinkCol = _db.GetCollection<Consumption>("drinks");
        var usersWithDrinks = drinkCol.FindAll().Select(d => d.User.Id).Distinct();

        var topList = new List<TopBacUser>();

        foreach (var id in usersWithDrinks)
        {
            var user = userCol.FindOne(u => u.Id == id);
            var bac = GetBacInternal(id, DateTime.Now, new List<Consumption>());
            
            topList.Add(new TopBacUser { Name = user.Name, Bac = bac.Bac });
        }

        return topList.OrderByDescending(t => t.Bac).ToList();
    }

    private BloodAlcoholLevel GetBacInternal(int userId, DateTime time, List<Consumption> drinks)
    {
        var userCol = _db.GetCollection<User>("users");
        var user = userCol.FindOne(u => u.Id == userId);
        
        if (!drinks.Any())
        {    
            var drinkCol = _db.GetCollection<Consumption>("drinks");
            drinks = drinkCol.Find(d => d.User.Id == userId).ToList();

            if (!drinks.Any())
            {
                return new BloodAlcoholLevel
                {
                    Bac = 0,
                    BacUpdated = time
                };
            }
        }

        // Only get drinks that were consumed before the given time. This allows history travel
        drinks = drinks.Where(d => d.TimeConsumed <= time).ToList();
        if (!drinks.Any())
        {
            return new BloodAlcoholLevel
            {
                Bac = 0,
                BacUpdated = time
            };
        }
        
        var firstDrinkTime = drinks.Min(d => d.TimeConsumed);
        var alcoholAmountSumInGrams = drinks.Sum(d => d.AlcoholMassInGrams);
        
        // bac = grams per decilitre
        var duration = (decimal) time.Subtract(firstDrinkTime).TotalHours;
        var bac = (alcoholAmountSumInGrams / ((user.Weight * 1000) * user.GetBodyWaterRatio()) * 100) - (duration * user.GetAlcoholMetabolizeRate());

        // the formula above can give negative values
        if (bac < 0)
        {
            bac = 0;
        }

        bac *= 10;
        
        return new BloodAlcoholLevel
        {
            Bac = Math.Round(bac, 3),
            BacUpdated = time
        };
    }

    public List<Consumption> GetDrinkList(int userId)
    {
        var userCol = _db.GetCollection<User>("users");
        var user = userCol.FindOne(u => u.Id == userId);

        var drinkCol = _db.GetCollection<Consumption>("drinks");
        var allUserDrinks = drinkCol.Find(d => d.User.Id == user.Id).ToList();

        return allUserDrinks;
    }

    public void DeleteDrink(int id, int userId)
    {
        var userCol = _db.GetCollection<User>("users");
        var user = userCol.FindOne(u => u.Id == userId);

        var drinkCol = _db.GetCollection<Consumption>("drinks");
        drinkCol.Delete(id);
    }
}