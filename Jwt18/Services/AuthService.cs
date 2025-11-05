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
    public class AuthService :IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public AuthService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context=context;   
        }
        public async Task<User?> RegisterAsync(UserDto request)
        {
            //check if the user exist in the database or not 
            if (await _context.users.AnyAsync(u => u.UserName == request.UserName))
            {
                return null;
            } //if this is no then we will create new user 
            var user = new User();
            user.UserName = request.UserName;
            user.PasswordHash= new PasswordHasher<User>().HashPassword(user, request.Password);
            //here we did the <user because the hash will be saved in the user class, Yani hasher ko bata rahe ho ki tum kis model ke liye password hash kar rahe ho.
            //here the request.password will come from the user 

            //2 user kya hai?
            // Ye ek User class ka object hai.
            // Hasher ko ye chahiye, kyunki kuch hashing algorithms user ke kuch details (jaise username, salt, type info, ya metadata) bhi internally use karte hain hash banane ke liye.
            //Aur verify karte time bhi same user object lagta hai — warna hash verify nahi hoga.
            await _context.users.AddAsync(user); //this line adds the data to the database but do not save it    
            await _context.SaveChangesAsync(); //this will save it 
            //here this is important 
            return user;
        }

        //the await addasync and the savechangesasync is only needed during the register time not in the login time 



        public async Task<string?> LoginAsync(UserDto request)
        {
            User? user=await _context.users.FirstOrDefaultAsync(u=>u.UserName == request.UserName);
            if (user is null)
            {
                return null; // if comes null it means that the user is not present in the database 
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)==PasswordVerificationResult.Failed)
            {
                return null; //if this is null or failed that means the user entered the wrong password, the password hash do not match. This also means that if this condition do not fullfill that means the entered password is correct and the token is now ready to generate
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));
            //basically ek security key bana rahi hai (object form me),
            //jo JWT ke signing aur verifying dono ke liye use hoti hai.
            //the above line is converting a string into bytes


            //JWT token ke andar ek signature hota hai.
            //Us signature ko banane ke liye ek secret key chahiye hoti hai —

            //here we are creating signing credential
            //Matlab ye line me hum batate hain kaunsa key aur kaunsa algorithm use karke JWT ka signature generate karna hai.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                //the name of the token provider basically a server id
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
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
