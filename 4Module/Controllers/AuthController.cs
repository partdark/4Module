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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtService _jwtService;

        public AuthController (UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }


        [HttpPost("/register")]
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

        [HttpPost("/login")]
        public async Task<ActionResult<JwtSecurityToken>> Login([FromBody] LoginUserDto loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user == null) { return Ok(new LoginUserResponseDto( false, "Wrong User Email")); }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);
            if (result.Succeeded) {
               var token = _jwtService.GenerateToken(user); 
                return Ok(token);
            }
            else
            {
                return Ok(new LoginUserResponseDto(false, "Wrong password"));
            }

        }
    }
}
