using Jwt18.Entities;
using Jwt18.Models;
using Jwt18.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Jwt18.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        //public static User user=new User(); this is the older way 
        //private static User user = new();//in future we will use the entity framework 
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
           _authService = authService;  
        }
        [HttpPost("register")]
        public async Task<ActionResult<User?>> Register(UserDto request)
        //from the frontend the username and the password will come  
        //Tu usse ek User entity banayega aur password ko hash karega.
        {
            var user=await _authService.RegisterAsync(request);
            if (user == null) { return BadRequest("The user already exsist in the database"); }
            return Ok(user);
        }



        [HttpPost("login")]
        public async Task<ActionResult<string>> login(UserDto request)
        //this will return a string,(baad me ye JWT token hoga).
        {
            
            var user=await _authService.LoginAsync(request);
            if (user == null) { return BadRequest("Either the username is wrong or the password is wrong check again"); }
            return Ok(user);
        }

        [HttpGet("test-endpoint")]
        [Authorize]
        public ActionResult Get()
        {
            return Ok();
        }
    }
}
