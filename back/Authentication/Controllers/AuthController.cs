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
         * This route checks for the existence of UserLoginCookie.
         * No matter the state of the cookie, this route returns status code 200.
         * The payload is '1' if the cookie exists and is not expired, or '0' if it expired or it does not exist
         */
        [HttpGet]
        [Route("isLogged")]
        [Produces("application/json")]
        public IActionResult IsLogged()
        {
            try
            {
                var cookie = HttpContext.User.Claims.First(c => c.Type == "Username").Value.ToString();
                if (cookie != String.Empty)
                {
                    return Ok(1);
                }
                else
                {
                    return Ok(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok(0);
            }
        }
    }
}
