using Jwt18.Data;
using Jwt18.Entities;
using Jwt18.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt18.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) 
    {
        public async Task<User?> RegisterAsync(UserDto request)
        {
            var user = new User();
            //check if the user exist in the database or not 
            if (await context.users.AnyAsync(u => u.UserName == request.UserName))
            {
                return null;
            }
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            return user;
        }



        public async Task<string?> LoginAsync(UserDto request)
        {
            User? user=await context.users.FirstOrDefaultAsync(u=>u.UserName == request.UserName);
            if (user is null)
            {
                return null; 
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)==PasswordVerificationResult.Failed)
            {
                return null;
            }
            string token = CreateToken(user);
            return token;
        }



        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName) 
                //name:username here the name is the key and the username is the value provided by the user 
                //claim is inbuilt in system.security.claim, it is a key value pair
                //here the claimtypes.name is prebuilt by the microsoft, and the user.username is what we are assigning there 
            };


            //creating a secret key 
            //UTF-8 ek encoding format hai jo text (characters) ko bytes (numbers) me convert karta hai.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            //basically ek security key bana rahi hai (object form me),
            //jo JWT ke signing aur verifying dono ke liye use hoti hai.
            //the above line is converting a string into bytes


            //JWT token ke andar ek signature hota hai.
            //Us signature ko banane ke liye ek secret key chahiye hoti hai —

            //here we are creating signing credential
            //Matlab ye line me hum batate hain kaunsa key aur kaunsa algorithm use karke JWT ka signature generate karna hai.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                //the name of the token provider basically a server id
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                //audience tells for whom this token is created for 
                claims: claims,
                //here the first one is the inbuilt claims and the second one is the data the claims list contains 
                expires: DateTime.UtcNow.AddDays(1),
                //every token has it expiry once it get expired the user have to do the login again not the register but only the login 
                signingCredentials: creds
                //ek key bnate hai us key ki help se sign karte hai token ko then the token is finally passed   
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            //this is the final step in which the token is created and it is converted into the string format 
            //Ab JwtSecurityTokenHandler() aaya aur bola: “Okay bhai, mai is sabko ek proper JWT format me pack kar deta hu.

            //WriteToken() method us data ko header + payload + signature format me encode karta hai aur ek long JWT string bana ke deta hai
        }

    }
}
