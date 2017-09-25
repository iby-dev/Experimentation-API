using MongoDB.Driver;

namespace Experimentation.Domain
{
    public interface IDataContext
    {
        IMongoDatabase Database { get; set; }
    }
}