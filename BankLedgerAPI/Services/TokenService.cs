using BankLedgerAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Services
{
    public class TokenService
    {
        private readonly DataContext _context;

        public TokenService(DataContext context)
        {
            _context = context;
        }

        public bool IsTokenBlackListed(Token token)
        {
            return _context.JWTBlacklist.Contains(token);
        }

        public bool IsTokenBlackListed(SecurityToken token)
        {
            return _context.JWTBlacklist.Any(Token => Token.JWTToken == token);
        }

        public void BlackListToken(Token token)
        {
            if (!IsTokenBlackListed(token))
            {
                _context.JWTBlacklist.Add(token);
                _context.SaveChanges();
            }
        }
    }
}
