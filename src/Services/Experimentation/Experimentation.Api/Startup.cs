using System;
using Experimentation.Api.Filters;
using Experimentation.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Experimentation.Api
{
    public class Startup
    {
        // PROPERTIES
        private readonly IConfiguration _configuration;

        // CONSTRUCTORS
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // METHODS
        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(new ValidateActionParameters());
                    options.Filters.Add(new CheckModelState());
                })
                .AddControllersAsServices();
            services.AddOptions();
            services.Configure<DataContextSettings>(_configuration);
            services.AddSwaggerSupport();
            return services.AddAutofacSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IApplicationLifetime lifetime)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseStaticFiles();

            app.UseMvc();
            app.UseSwagger().UseSwaggerUi();

            app.UseMvcWithDefaultRoute();

            lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
        }
    }
}
