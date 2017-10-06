﻿using System;
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
        private readonly ILoggerFactory _logFac;

        // CONSTRUCTORS
        public Startup(IConfiguration configuration, ILoggerFactory logFac)
        {
            _configuration = configuration;
            _logFac = logFac;
        }

        // METHODS
        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(new ValidateActionParameters());
                    options.Filters.Add(new CheckModelState());
                    options.Filters.Add(new GlobalExceptionFilter(_logFac));
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
            IApplicationLifetime lifetime)
        {
            _logFac.AddSerilog();
            _logFac.AddConsole(_configuration.GetSection("Logging"));
            _logFac.AddDebug();

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
