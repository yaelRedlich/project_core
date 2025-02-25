using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_core.Services;


namespace project.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase{
        public AdminController() { }
        [HttpPost]
        [Route("[action]")]
        public ActionResult <String> Login ([FromBody] User User){
            if(User.Username!="yael" || User.Password!="327804233"){
                 return Unauthorized();
            }
            var claims = new List<Claim>
            {
                new Claim("type", "Admin"),
                new Claim("UserId", "0"),
                new Claim("name", "yael"),
            };
            var token =TokenService.GetToken(claims);
            return new OkObjectResult(TokenService.WriteToken(token));
        }

    }
}
