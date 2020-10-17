using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankLedgerAPI.Models;
using BankLedgerAPI.Services;
using BankLedgerAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BankLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserService _userService;
        private readonly AccountService _accountService;
        private readonly TokenService _tokenService;

        public AccountController(DataContext context)
        {
            _context = context;
            _userService = new UserService(_context);
            _accountService = new AccountService(_context);
            _tokenService = new TokenService(_context);
        }

        //[Authorize]
        [HttpPost("open")]
        public async Task<IActionResult> OpenAccount([FromHeader] string Authorization, [FromHeader] string accountNickname, [FromHeader] string type) {
            
            try {
                //TODO middleware/filter would be better
                if (TokenUtils.IsValidFormatTokenString(Authorization) && TokenUtils.ValidateToken(Authorization, out SecurityToken token) && !_tokenService.IsTokenBlackListed(token)) {
                    int id = TokenUtils.GetUserIDFromToken(Authorization, token);
                    Models.User user = _userService.GetById(id);
                    
                    if (user != null && ValidationUtils.IsMatch(Settings.AccountNameCriterion, accountNickname,Settings.AccountNameMinLength, Settings.AccountNameMaxLength)) {
                        
                        AccountType accountType = (AccountType)Enum.Parse(typeof(AccountType), type, true);
                        AccountHolder primaryAccountHolder = new AccountHolder()
                        {
                            AccountHolderType = AccountHolderType.Primary,
                            Holder = user
                        };
                        Account account = _accountService.OpenAccount(primaryAccountHolder, accountType, accountNickname);

                        // TODO move this to account service instead
                        _context.AccountHolders.Add(primaryAccountHolder);
                        _context.Accounts.Add(account);
                        _context.SaveChanges();

                        return Ok(String.Format("New {0} account created with nickname {1}",accountType.ToString(), accountNickname));
                    }
                    return BadRequest("Cannot create account using account type and nickname supplied.");
                }
                else
                {
                    return BadRequest("Unable to authorize account creation. Check credentials and try again.");
                }
            }

            catch (Exception ex) {
                // TODO put more informative exception message to log
                System.Console.WriteLine(ex.Message);
                return BadRequest("A problem occurred. A new account was not created.");
            }
         }
    }
}