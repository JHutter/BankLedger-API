using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using BankLedgerAPI.Models;
using System.Security.Cryptography;
using IdentityModel.Client;
using BankLedgerAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using BankLedgerAPI.Services;

/* Session Controller
 * endpoints  
 *      POST api/session/login
 *      POST api/session/logout
 */
namespace BankLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // TODO implement using filters
    public class SessionController : Controller
    {
        private readonly DataContext _context;
        private readonly TokenService tokenService;

        public SessionController(DataContext context)
        {
            _context = context;
            tokenService = new TokenService(_context);
        }

        /*[AllowAnonymous]*/
        [HttpPost("login")]
        public ActionResult Login([FromHeader] string username, [FromHeader] string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Unable to log in.");
            }

            var user = _context.Users.Where(u => u.Username == username).FirstOrDefault();

            if (user != null)
            {
                string hashedFromUserPass = Convert.ToBase64String(HashUtils.HashWithSuppliedSalt(password, user.Salt));
                if (hashedFromUserPass.Equals(Convert.ToBase64String(user.Password)))
                {
                    HttpContext.Response.Headers.Append("Authorization", TokenUtils.GenerateToken(user));
                    return Ok("login successful");
                }

            }
            
            return BadRequest("Unable to authenticate the request");
        }

        [HttpPost("logout")]
        //[Authorize]
        public ActionResult Logout([FromHeader] string Authorization)
        {
            // add token to blacklist
            if (!string.IsNullOrWhiteSpace(Authorization) && Authorization.Contains("Bearer "))
            {
                string tokenText = Authorization.Split(" ")[1];

                // no need to add to blacklist if it's not valid
                if (TokenUtils.ValidateToken(tokenText, out SecurityToken securityToken))
                {
                    Token tempToken = new Token()
                    {
                        Expiration = securityToken.ValidTo,
                        TokenString = tokenText,
                        JWTToken = securityToken
                    };

                    tokenService.BlackListToken(tempToken);
                }

                HttpContext.Response.Headers.Append("Authorization", " ");
                return Ok("logout successful");
            }
            else
            {
                return BadRequest("missing session information");
            }

        }

    }
}
