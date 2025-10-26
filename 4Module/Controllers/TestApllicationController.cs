using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace _4Module.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class TestApllicationController : ControllerBase
    {
        private readonly MySettings _settings;

        public TestApllicationController(IOptions<MySettings> setting)
        {
            _settings = setting.Value;
        }
        /// <summary>
        /// Get setting from MySettings
        /// </summary>
        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            await Task.Delay(1000);
            return Ok(new
            {
                ApplicationName = _settings.ApplicationName,
                MaxBooksPerPage = _settings.MaxBooksPerPage,
                ApiSettings = _settings.ApiSettings
            });
        }

        /// <summary>
        /// middleware check
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpGet("error")]
        public async Task<IActionResult> DivideByZero()
        {
            await Task.Delay(1000);

            throw new InvalidOperationException();
        }

    }
}
