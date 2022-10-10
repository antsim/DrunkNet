using DrunkNet.API.Models;
using LiteDB;

namespace DrunkNet.API.Services;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly LiteDatabase _db;
    
    public UserService(IConfiguration configuration, ILiteDbContext liteDbContext)
    {
        _configuration = configuration;
        _db = liteDbContext.Database;
    }
    
    public int GetOrAddByExternalId(string externalId)
    {
        var col = _db.GetCollection<User>("users");
        var user = col.FindOne(u => u.ExternalAuthId == externalId);

        if (user == null)
        {
            user = new User {ExternalAuthId = externalId};
            col.Insert(user);
        }

        return user.Id;
    }

    public User GetProfile(int id)
    {
        var col = _db.GetCollection<User>("users");
        var user = col.FindOne(u => u.Id == id);

        return user;
    }

    public User UpdateProfile(User user, int id)
    {
        var col = _db.GetCollection<User>("users");
        var dbUser = col.FindOne(u => u.Id == id);

        dbUser.Weight = user.Weight;
        dbUser.Gender = user.Gender;
        dbUser.Name = user.Name;

        col.Update(dbUser);

        return dbUser;
    }
}