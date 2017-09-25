using System.Collections.Generic;
using System.Threading.Tasks;
using Experimentation.Domain.Entities;

namespace Experimentation.Logic.Directors
{
    public interface IFeaturesDirector
    {
        Task<IEnumerable<Feature>> GetAllFeatures();
        Task<Feature> GetFeatureById(string id);
        Task<Feature> AddNewFeature(Feature toAdd);
        Task<Feature> GetFeatureByFriendlyId(int id);
        Task UpdateFeature(Feature toUpdate);
        Task DeleteFeature(string id);
        Task<Feature> GetFeatureByName(string name);
    }
}