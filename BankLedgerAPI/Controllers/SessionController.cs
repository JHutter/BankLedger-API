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

        public SessionController(DataContext context)
        {
            _context = context;
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
                    return Ok();
                }

            }
            
            return BadRequest("Unable to authenticate the request");
        }



        //// TODO
        //[HttpPost("logout")]
        ////[Authorize]
        //public ActionResult Logout([FromHeader] string jwt_token)
        //{
        //    // add token to blacklist
        //    //string tokenString =
        //    if (!string.IsNullOrWhiteSpace(jwt_token))
        //    {
        //        _context.JWTBlacklist.Add(jwt_token);
        //        _context.SaveChanges();

        //        //var response = new TokenResponse("0", string.Empty);
        //        var response = new TokenResponse();
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
            
        //}

    }
}
