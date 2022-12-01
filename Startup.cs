using Amazon.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog.Config;
using NLog.Layouts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.AWS.Logger;
using Amazon.Util;
using Amazon.Runtime.CredentialManagement;
using NLog.Web;

namespace EcsNetTestAwsLogging
{
    public class Startup
    {
        Logger logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EcsNetTestAwsLogging", Version = "v1" });
            });

            services.AddDefaultAWSOptions(s => s.GetService<IConfiguration>().GetAWSOptions());

            // Confifures the ASP.NET Core logging to write logs to the log group provided in your CDK code
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddAWSProvider();       
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcsNetTestAwsLogging v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            logger.Debug("Configure complete.");
        }
    }
}
