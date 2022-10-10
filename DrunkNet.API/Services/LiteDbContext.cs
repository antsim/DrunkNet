using LiteDB;

namespace DrunkNet.API.Services;

public class LiteDbContext : ILiteDbContext
{
    public LiteDatabase Database { get; }

    public LiteDbContext(IConfiguration configuration)
    {
        Database = new LiteDatabase(configuration["LiteDb:DbLocation"]);
    }
}