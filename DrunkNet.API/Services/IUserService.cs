using DrunkNet.API.Models;

namespace DrunkNet.API.Services;

public interface IUserService
{
    int GetOrAddByExternalId(string externalId);
    User GetProfile(int id);
    User UpdateProfile(User user, int id);
    
}