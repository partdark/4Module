using Application;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace _4Module.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class TestApllicationController : ControllerBase
    {
        private readonly MySettings _settings;
        private readonly IBookService _bookRepository;

        public TestApllicationController(IOptions<MySettings> setting, IBookService bookRepository)
        {
            _settings = setting.Value;
            _bookRepository = bookRepository;
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


        [HttpGet("PublicGetHttpClinet")]
        public async Task<IActionResult> PublicGet()
        {
            var result =  await _bookRepository.PublicGet();

            return Ok(result);
        }

       
    }
}
