using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcsNetTestAwsLogging.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }

    [ApiController]
    [Route("")]
    [Route("[controller]")]
    public class EnvListController : ControllerBase
    {
        private readonly ILogger<EnvListController> _logger;

        public EnvListController(ILogger<EnvListController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public System.Collections.IDictionary Get()
        {
            var l = NLog.LogManager.GetCurrentClassLogger();
            l.Trace("l Started Env List");
            _logger.LogTrace("Started Env List");
            _logger.LogInformation("Started Env List - Information");
            var outEnv = Environment.GetEnvironmentVariables();
            _logger.LogTrace("Ended Env List");
            return outEnv;
        }
    }

    [ApiController]
    [Route("[controller]/{envvar}")]
    public class EnvController : ControllerBase
    {
        private readonly ILogger<EnvController> _logger;

        public EnvController(ILogger<EnvController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get(string envvar)
        {
            return Environment.GetEnvironmentVariable(envvar);
        }
    }
}
