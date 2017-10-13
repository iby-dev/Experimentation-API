using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Experimentation.Configuration.Ioc;
using Experimentation.Persistence.Ioc;
using Experimentation.Logic.Ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Experimentation.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSwaggerSupport(this IServiceCollection services, IConfiguration configuration)
        {
            var pathToDoc = configuration["Swagger:FileName"];

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Contact = new Contact { Name = "Ibrar Mumtaz"},
                    Title = "Experimentation HTTP API",
                    Version = "v1",
                    Description = "A Rest service that provides http responses to feature string requests.",
                    TermsOfService = "To be used by Age and Pure code only"
                });
            });
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, pathToDoc);
                options.IncludeXmlComments(xmlPath);
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