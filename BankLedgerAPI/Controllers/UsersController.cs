using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankLedgerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        public UsersController(List<User> users)
        {
            _context.Users.AddRange(users);
            _context.SaveChanges();
        }



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
    }
}