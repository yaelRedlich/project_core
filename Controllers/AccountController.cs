using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using project;
using project.Interfaces;
using project_core.Helpers;
using project_core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace project_core
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        private IUserService userService;
       private readonly List<User> listUsers;
        public AccountController(IUserService userService)
        {
            listUsers = FileHelper<User>.ReadFromJson();
            this.userService = userService;
        }



        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto model)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { "615496630298-8lb43opj1vhse725dql9aucvh1369th0.apps.googleusercontent.com" } 
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.Token, settings);
                var user = new User
                {
                    Email = payload.Email,
                    Username = payload.GivenName
                };

                var token = GenerateJwtToken(user);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid Google token.");
            }
        }
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SXkSqsKyNUyvGbnyjyjuykfFHJq8zQzhLW7mPmHbnZZ"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            string email = user.Email;
            var findUser = listUsers.FirstOrDefault(u => u.Email == email);
            if (findUser == null)
            {
                return "User not found in system.";
            }

            var claims = new List<Claim>
            {
                new Claim("UserId", findUser.UserId.ToString()),
                new Claim("userName", findUser.Username),
                new Claim("isAdmin", findUser.isAdmin ? "true" : "false")
            };

             var token=TokenService.GetToken(claims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Failure != null)
            {
                System.Console.WriteLine("Authentication Error: " + result.Failure.Message);
            }
            var claims = result.Principal.Identities.FirstOrDefault()
                .Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });
            return Json(claims);
        }
    }
}

public class GoogleLoginDto
{
    public string Token { get; set; }
}



