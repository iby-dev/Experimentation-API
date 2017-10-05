using System.Collections.Generic;
using Experimentation.Domain.Entities;
using Experimentation.Logic.ViewModels;

namespace Experimentation.Logic.Mapper
{
    public class FeatureViewModelMapper : IDtoToEntityMapper<BaseFeatureViewModel, Feature>
    {
        public Feature Map(BaseFeatureViewModel model)
        {
            return new Feature
            {
                Name = model.Name,
                FriendlyId = model.FriendlyId,
                BucketList = model.BucketList,
            };
        }
    }
}