using Application.DTO;
using Applications.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace _4Module.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        
        private readonly JwtService _jwtService;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController (UserManager<IdentityUser> userManager,  JwtService jwtService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
           
            _jwtService = jwtService;

            _roleManager = roleManager;
        }
        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name,
                Claims = claims
            });
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
                if (!await _roleManager.RoleExistsAsync(createUserDto.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(createUserDto.Role));
                }
                await _userManager.AddToRoleAsync(user, createUserDto.Role);
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("DateOfBirth", createUserDto.DateOfBirth.ToString("yyyy-MM-dd")));
                return Ok(new ResponseUserDto(user.Id, user.UserName, user.Email));
            }
            var errors = registerUser.Errors.Select(e => e.Description).ToList();
            Console.WriteLine($"Registration failed: {string.Join(", ", errors)}");

            return BadRequest(new
            {
                Message = "Registration failed",
                Errors = errors
            });

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDto loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Mail);
            if (user == null) { return Ok("Wrong User Email"); }

            var result = await _userManager.CheckPasswordAsync(user, loginUser.Password);
            if (result) {
                var claims = await _userManager.GetClaimsAsync(user);
                var dateOfBirthClaim = claims.FirstOrDefault(c => c.Type == "DateOfBirth");
                var dateOfBirth = dateOfBirthClaim != null ? DateOnly.Parse(dateOfBirthClaim.Value) : DateOnly.MinValue;
                var token = _jwtService.GenerateToken(user, _userManager, dateOfBirth); 
                return Ok( new { token } );
            }
            else
            {
                return Ok( "Wrong password");
            }

        }
    }
}
