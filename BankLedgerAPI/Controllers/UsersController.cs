using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;


        //public UsersController()
        //{
        //}

        public UsersController(DataContext context)
        {
            _context = context;
        }


        //public UsersController(List<User> users)
        //{
        //    _context.Users.AddRange(users);
        //    _context.SaveChanges();
        //}


        // TODO change this, obvs don't want to spit out all users with a simple GET
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //var currentUser = _context.CurrentUser;
            //if ()
            var users = _context.Users;


            var response = users.Select(u => new
            {
                userId = u.Id,
                username = u.Username,
                firstName = u.FName,
                lastName = u.LName,

            });

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromHeader] string username, [FromHeader] string password, string fname, string lname)
        {
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(fname) || String.IsNullOrWhiteSpace(lname))
            {
                return BadRequest("Invalid params for user registration.");
            }

            var users = _context.Users.Where(u => u.Username == username);

            if (users.Count() == 0)
            {
                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    var sha1data = sha1.ComputeHash(Encoding.ASCII.GetBytes(password));
                    var testUser = new Models.User
                    {
                        Username = username,
                        FName = fname,
                        LName = lname,
                        Password = sha1data
                    };

                    _context.Users.Add(testUser);
                    _context.SaveChanges();

                    return Ok(String.Format("New user {0} created", username));
                }

            }
            //else
            //{
            //    return BadRequest(users.Count().ToString());
            //}

            return BadRequest("Username already exists.");
        }
    }
}