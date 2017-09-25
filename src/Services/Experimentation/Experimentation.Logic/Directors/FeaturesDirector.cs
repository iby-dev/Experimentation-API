using System.Collections.Generic;
using System.Threading.Tasks;
using Experimentation.Domain.Entities;
using Experimentation.Persistence.Repositories;

namespace Experimentation.Logic.Directors
{
    public class FeaturesDirector : IFeaturesDirector
    {
        private readonly FeaturesRepository _repository;

        public FeaturesDirector(BaseRepository<Feature> repository)
        {
            _repository = repository as FeaturesRepository;
        }

        public Task<IEnumerable<Feature>> GetAllFeatures()
        {
            return _repository.FindAllAsync(x => true);
        }

        public Task<Feature> GetFeatureById(string id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<Feature> GetFeatureByName(string name)
        {
            return _repository.GetByNameAsync(name);
        }

        public Task<Feature> GetFeatureByFriendlyId(int id)
        {
            return _repository.GetByFriendlyIdAsync(id);
        }

        public Task<Feature> AddNewFeature(Feature toAdd)
        {
            return _repository.SaveAsync(toAdd);
        }

        public Task UpdateFeature(Feature toUpdate)
        {
             return _repository.UpdateAsync(toUpdate);
        }

        public Task DeleteFeature(string id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}
