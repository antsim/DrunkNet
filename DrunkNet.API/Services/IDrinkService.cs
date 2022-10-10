using DrunkNet.API.Models;
using DrunkNet.API.Request;
using DrunkNet.API.Response;

namespace DrunkNet.API.Services;

public interface IDrinkService
{
    BloodAlcoholLevel AddDrink(DrinkRequest request, int userId);
    BloodAlcoholLevel GetBac(int userId, List<Consumption> drinks);
    List<BloodAlcoholLevel> GetBacHistory(int userId);
    List<TopBacUser> GetTopList();
    List<Consumption> GetDrinkList(int userId);
    void DeleteDrink(int id, int userId);
}