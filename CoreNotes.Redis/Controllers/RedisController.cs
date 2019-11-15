using System;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;

namespace CoreNotes.Redis.Controllers
{
    [Route("[controller]/[action]")]
    public class RedisController : ControllerBase
    {
        private readonly IEasyCachingProviderFactory _factory;

        public RedisController(IEasyCachingProviderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public string Handle()
        {
            var provider = _factory.GetCachingProvider("RedisExample");

            //Set
            provider.Set("demo", "123", TimeSpan.FromMinutes(1));

            //Set Async
            // await provider.SetAsync("demo", "123", TimeSpan.FromMinutes(1));

            return provider.Get<string>("demo").Value;
        }
    }
}