using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PublishSubscribePattern.Abstractions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IEventsBroker eb;

        public WeatherForecastController(IEventsBroker eventsBroker)
        {
            this.eb = eventsBroker;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            async Task func1(string str)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                Console.WriteLine("subs 1 " + str);
            }
            async Task func2(string str)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                Console.WriteLine("subs 2 " + str);
            }
            var id1 = await eb.Subscribe<string>(func1);
            var id2 = await eb.Subscribe<string>(func2);

            await eb.Publish("test1");
            await eb.Publish("test2");
            await eb.Publish("test3");
            await eb.Publish("test4");

            await eb.Unsubscribe(id1);
            await eb.Unsubscribe(id2);

            return Ok(null);
        }
    }
}
