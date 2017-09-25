using System.Threading.Tasks;
using Experimentation.Domain;
using Experimentation.Domain.Entities;
using MongoDB.Driver;

namespace Experimentation.Persistence.Repositories
{
    public class FeaturesRepository : BaseRepository<Feature> 
    {
        private const string CollectionName = "Features";

        private readonly IDataContext _ctx;
        protected override IMongoCollection<Feature> Collection => _ctx.Database.GetCollection<Feature>(CollectionName);

        public FeaturesRepository(IDataContext context)
        {
            _ctx = context;
        }

        public virtual async Task<Feature> GetByNameAsync(string name)
        {
            return 
                await Retry(async () => await Collection.Find(x => x.Name.Equals(name)).FirstOrDefaultAsync());
        }
    }
}