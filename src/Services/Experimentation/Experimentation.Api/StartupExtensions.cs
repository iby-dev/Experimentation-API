using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Swagger.Model;
using Experimentation.Configuration.Ioc;
using Experimentation.Persistence.Ioc;
using Experimentation.Logic.Ioc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Experimentation.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSwaggerSupport(this IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
                options.SingleApiVersion(new Info
                {
                    Title = "Experimentation HTTP API",
                    Version = "v1",
                    Description = "A Rest service that provides http responses to feature string requests.",
                    TermsOfService = "To be used by Age and Pure code only"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Experimentation.Api.xml");
                options.IncludeXmlComments(xmlPath); ;
            });
            return services;
        }

        public static AutofacServiceProvider AddAutofacSupport(this IServiceCollection service)
        {
            var container = new ContainerBuilder();
            container.Populate(service);

            container.RegisterModule(new ConfigurationModule());
            container.RegisterModule(new PersistenceModule());
            container.RegisterModule(new LogicModule());

            return new AutofacServiceProvider(container.Build());
        }
    }
}