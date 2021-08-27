using Authentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDatabaseContext _context;

        public AuthController(AuthDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (authHeader != null)
            {
                try
                {
                    string encodedCredentials = authHeader.Substring("Basic.".Length).Trim();
                    string credentials = Encoding
                        .GetEncoding("iso-8859-1")
                        .GetString(Convert.FromBase64String(encodedCredentials));
                    int sepIndex = credentials.IndexOf(':');
                    string username = credentials.Substring(0, sepIndex);
                    string password = credentials.Substring(sepIndex + 1);

                    var users = _context.Users.Where(user => user.Username == username && user.Password == password);
                    if (users.Count() == 1)
                    {
                        var claims = new Claim[]
                        {
                        new Claim("ID", users.FirstOrDefault().UserId.ToString()),
                        new Claim("Username", users.FirstOrDefault().Username.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationHttpContextExtensions.SignInAsync(
                            HttpContext,
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity)).Wait();

                        return Ok("Logged in");
                    }

                    return new UnauthorizedResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return StatusCode(500);
                }
            }
            return StatusCode(501);
        }

        [HttpGet]
        [Route("logout")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }

        /*
         * The only way a user can use this route is if UserLoginCookie is stored in the browser
         * due to the 'Authorize' attribute, so no additional checks are required.
         * If the user is logged, this method returns status 200 with the username in the body.
         * This is how user info can be retrieved from the claims stored inside the cookie.
         */
        [HttpGet]
        [Route("isLogged")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult IsLogged()
        {
            return Ok(HttpContext.User.Claims.First(c => c.Type == "Username").Value.ToString());
        }
    }
}
