using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.CredentialManagement;
using Amazon.CloudWatch;
using NLog.AWS.Logger;
using NLog.Layouts;
using NLog.Config;
using LogLevel = NLog.LogLevel;

namespace EcsNetTestAwsLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            var outEnv = Environment.GetEnvironmentVariables();

            var awsTarget = new AWSTarget()
            {
                LogGroup = "dbTest",
                Region = "eu-west-2",
                // YOUR cred
                //Credentials = new Amazon.Runtime.ECSTaskCredentials(),
                Layout = new SimpleLayout
                {
                    Text = "${longdate} ${level:uppercase=true} ${machinename} ${message} ${exception:format=tostring}"
                }
            };
            var config = new LoggingConfiguration();
            config.AddTarget("aws", awsTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, awsTarget));
            LogManager.Configuration = config;

            IAmazonCloudWatch cwClient = new AmazonCloudWatchClient(Amazon.RegionEndpoint.EUWest2);
            logger.Info("Start Of Env Variables");
            logger.Info("----------------------");

            foreach (DictionaryEntry dictionaryEntry in outEnv)
            {
                logger.Info($"{dictionaryEntry.Key},{dictionaryEntry.Value}");
            }

            logger.Info("----------------------");
            logger.Info("End Of Env Variables");
            Console.WriteLine("init main from console writeline");
            logger.Debug("init main from log debug");

            CreateHostBuilder(args).Build().Run();

            logger.Debug("shutdown from log debug");
            NLog.LogManager.Shutdown();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                })
                .UseNLog();
    }
}
