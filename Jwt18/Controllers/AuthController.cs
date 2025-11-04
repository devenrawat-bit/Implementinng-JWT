using Jwt18.Entities;
using Jwt18.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Jwt18.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //public static User user=new User(); this is the older way 
        public static User user = new();
        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        //from the frontend the username and the password will come  
        //Tu usse ek User entity banayega aur password ko hash karega.
        {
            var hashedPassword = new PasswordHasher<User>()
                //here we did the <user because the hash will be saved in the user class, Yani hasher ko bata rahe ho ki tum kis model ke liye password hash kar rahe ho.
                .HashPassword(user, request.Password);
            //here the request.password will come from the user 

            //2 user kya hai?
            // Ye ek User class ka object hai.
            // Hasher ko ye chahiye, kyunki kuch hashing algorithms user ke kuch details (jaise username, salt, type info, ya metadata) bhi internally use karte hain hash banane ke liye.
            //Aur verify karte time bhi same user object lagta hai — warna hash verify nahi hoga.

            user.UserName = request.UserName;
            user.PasswordHash = hashedPassword;
            return Ok(user);
        }
    }
}
