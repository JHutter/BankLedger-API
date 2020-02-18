using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BankLedgerAPI.Models;
using BankLedgerAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace BankLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string emptyStringHash;

        public UsersController(DataContext context)
        {
            _context = context;
            emptyStringHash = Encoding.ASCII.GetString(HashUtils.HashPassword(string.Empty));
        }

        // TODO change this, obvs don't want to spit out all users with a simple GET
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("ok");
        }

        /// <summary>
        /// Registers a new user using the supplied user info and login info
        /// </summary>
        /// <param name="username">desired username, alphanumeric, 3-32 characters long</param>
        /// <param name="password">desired password</param>
        /// <param name="userData">json object with fname and lname</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromHeader] string username, [FromHeader] string password, [FromBody] JObject userData)
        {
            var user = userData.ToObject<User>();
            Regex regex = new Regex(@"^[0-9a-zA-Z]{3,32}$");
            //string emptyStringHash = Encoding.ASCII.GetString(HashUtil.HashPassword(string.Empty));
            // TODO add checks to password lengths and complexity

            if (String.IsNullOrWhiteSpace(username) || password.Equals(emptyStringHash) || String.IsNullOrWhiteSpace(user.FName) || String.IsNullOrWhiteSpace(user.LName))
            {
                return BadRequest("Invalid params for user registration.");
            }

            if (!regex.IsMatch(username))
            {
                return BadRequest("Illegal characters in username. Alphanumeric only.");
            }

            var users = _context.Users.Where(u => u.Username == username);

            if (users.Count() == 0)
            {
                Tuple<byte[], string> hashAndSalt = HashUtils.HashWithNewSalt(password);
                    
                var testUser = new Models.User
                {
                    Username = username,
                    FName = user.FName,
                    LName = user.LName,
                    Password = hashAndSalt.Item1,
                    Salt = hashAndSalt.Item2
                };

                _context.Users.Add(testUser);
                _context.SaveChanges();

                return Ok(String.Format("New user {0} created", username));

            }
            return BadRequest("Username already exists.");
        }

    }
}