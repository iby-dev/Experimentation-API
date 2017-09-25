using Experimentation.Configuration;
using Experimentation.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Experimentation.Persistence
{
    public class DataContext : IDataContext
    {
        public IMongoDatabase Database { get; set; }

        public DataContext(IOptions<DataContextSettings> options)
        {
            var url = new MongoUrl(options.Value.ConnectionString);
            var client = new MongoClient(url);
            Database = client.GetDatabase(options.Value.Database);
        }
    }
}