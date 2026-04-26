using EmpMngtAPI.DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace EmpMngtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // API : Application Programming Interface 
        // Type : REST, SOAP.. etc.
        private readonly AppDbContext _authContext;
        private readonly IConfiguration _configuration;
        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _authContext = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserTbl userObj)
        {
            if (userObj == null)
                return BadRequest();
            //check username
            if (await CheckUsernameExistAsync(userObj.UserName))
                return BadRequest(new { Message = "UserName already exist" });

            // check email
            if (await CheckEmailExistAsync(userObj.UserName))
                return BadRequest(new { Message = "Email already exist" });

            // Check password strength
            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                //return BadRequest($"Pass {pass}");
                return BadRequest(new { Message = pass });


            //userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            //userObj.Password = userObj.Password;
            userObj.Role = "User";

            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Status = 200,
                Message = "User Registered"
            });
        }


        private Task<bool> CheckUsernameExistAsync(string? username)
            => _authContext.Users.AnyAsync(x => x.UserName == username);

        private Task<bool> CheckEmailExistAsync(string? email)
           => _authContext.Users.AnyAsync(x => x.Email == email);

        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new StringBuilder();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
                sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special charcter" + Environment.NewLine);
            return sb.ToString();
        }

        //public static string HashPassword(string password)
        //{
        //    byte[] salt; 
            
        //}
    }
}
