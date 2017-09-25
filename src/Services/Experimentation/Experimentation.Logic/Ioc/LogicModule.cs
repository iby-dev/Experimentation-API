using Autofac;
using Experimentation.Domain.Entities;
using Experimentation.Logic.Directors;
using Experimentation.Logic.Mapper;
using Experimentation.Logic.ViewModels;

namespace Experimentation.Logic.Ioc
{
    public class LogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FeaturesDirector>().As<IFeaturesDirector>();
            builder.RegisterType<FeatureViewModelMapper>().As<IDtoToEntityMapper<BaseFeatureViewModel, Feature>>();
        }
    }
}