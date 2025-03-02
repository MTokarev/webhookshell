using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Webhookshell.Interfaces;
using Webhookshell.Options;
using Webhookshell.Services;
using Webhookshell.Validators;

namespace Webhookshell
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ScriptOptions>(options =>
            {
                Configuration.GetSection("Scripts").Bind(options);
            });
            
            services.AddControllers();
            services.AddScoped<IScriptRunnerService, ScriptRunner>();
            services.AddScoped<IHandlerDispatcher, HandlerDispatcher>();
            services.AddScoped<IScriptValidationService, ScriptValidationService>();

            // Register validators
            // The order is matter, if the first validator fails
            // the service return validation errors and stop further validation.
            // This was made like that because in some cases when validator 1 is failed
            // then it does not make sense to run the validator 2 because it might depend on the 1st one.
            services.AddScoped<IScriptValidator, HttpTriggerValidator>();
            services.AddScoped<IScriptValidator, IPAddressValidator>();
            services.AddScoped<IScriptValidator, KeyValidator>();
            services.AddScoped<IScriptValidator, TimeValidator>();
            
            services.AddSwaggerGen(options =>
            {
                // Get the XML file path of the documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
                // Include the XML comments
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebhookShell Project");
                options.RoutePrefix = string.Empty;
            });

            app.UseExceptionHandler("/error");
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}