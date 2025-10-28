using Application.DTO;
using Applications.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace _4Module.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        
        private readonly JwtService _jwtService;

        public AuthController (UserManager<IdentityUser> userManager,  JwtService jwtService)
        {
            _userManager = userManager;
           
            _jwtService = jwtService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<ResponseUserDto>> RegisterUser([FromBody] CreateUserDto createUserDto )
        {
            if (createUserDto.Password != createUserDto.ConfirmPassword)
            {
                return BadRequest("The passwords don't match");
            }
            var user = new IdentityUser
            {
                UserName = createUserDto.Name,
                Email = createUserDto.Email
            };
            var registerUser = await _userManager.CreateAsync(user, createUserDto.Password);
            if (registerUser.Succeeded)
            {
                return Ok(new ResponseUserDto(user.Id, user.UserName, user.Email));
            }
            else return BadRequest("Register error");

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDto loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user == null) { return Ok("Wrong User Email"); }

            var result = await _userManager.CheckPasswordAsync(user, loginUser.Password);
            if (result) {
               var token = _jwtService.GenerateToken(user); 
                return Ok(token);
            }
            else
            {
                return Ok( "Wrong password");
            }

        }
    }
}
