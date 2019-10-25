using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Odp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<RoleController> _logger;

        public RoleController(ILogger<RoleController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdministratorRole")]
        public IEnumerable<MyDate> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new MyDate
            {
                Date = DateTime.Now.AddDays(index)
            })
            .ToArray();
        }
    }
}
