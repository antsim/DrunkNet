using LiteDB;

namespace DrunkNet.API.Services;

public interface ILiteDbContext
{
    LiteDatabase Database { get; }
}