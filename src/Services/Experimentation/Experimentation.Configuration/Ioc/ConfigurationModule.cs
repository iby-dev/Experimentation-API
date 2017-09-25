using Autofac;

namespace Experimentation.Configuration.Ioc
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataContextSettings>().SingleInstance();
        }
    }
}