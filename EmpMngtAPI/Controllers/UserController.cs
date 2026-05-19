using EmpMngtAPI.DataModel;
using EmpMngtAPI.Helper;
using EmpMngtAPI.Model.RequestModel;
using EmpMngtAPI.Model.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDTO userobj)
        {
            if (userobj == null)
                return BadRequest();
            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.UserName == userobj.UserName);
            if (user == null)
                return BadRequest(new { Message = "Invalid UserName" });

            if (!PasswordHasher.VerifyPassword(userobj.Password, user.Password))
                return BadRequest(new { Message = "Password is InCorrect" });
            user.Token = CreateJWT(user);
            var newAccessToken = user.Token;
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _authContext.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });

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


            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";

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

        private string CreateJWT(UserTbl user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(

                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(45),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUserTbl = _authContext.Users.Any(a => a.RefreshToken == refreshToken);
            if (tokenInUserTbl)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> ResetPasswordEmail(string email)
        {
            try
            {
                var user = await _authContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Email Doesn't Exist!"
                    });

                }
                var tokenBytes = RandomNumberGenerator.GetBytes(64);
                var emailToken = Convert.ToBase64String(tokenBytes);
                user.ResetPasswordToken = emailToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(15);
                string from = _configuration["EmailSetting:From"];
                var emailModel = new EmailModel(email, "Reset Password", EmailBody.EmailStringBody(email, emailToken));

                //_emailService.SendEmail(emailModel);
                _authContext.Entry(user).State = EntityState.Modified;
                await _authContext.SaveChangesAsync();
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Email Sent Successfully !"
                });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var newToken = resetPasswordDTO.EmailToken.Replace(" ", "+");
            var user = await _authContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == resetPasswordDTO.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email Doesn't Exist!"
                });
            }
            var tokenCode = user.ResetPasswordToken;
            DateTime? emailTokenExpiry = user.ResetPasswordExpiry;
            if (tokenCode != resetPasswordDTO.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new

                {
                    StatusCode = 400,
                    Message = "Invalid reset link"
                });
            }
            user.Password = PasswordHasher.HashPassword(resetPasswordDTO.NewPassword);
            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Password Reset Successfully"
            });
        }
    }
}
