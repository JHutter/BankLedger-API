using BankLedgerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Services
{
    public class AccountService
    {
        private readonly DataContext _context;

        public AccountService(DataContext context)
        {
            _context = context;
        }

        public Account OpenAccount(AccountHolder primaryAccountHolder, AccountType accountType, String nickname)
        {
            List<AccountHolder> accountHolders = new List<AccountHolder>();
            accountHolders.Add(primaryAccountHolder);

            return new Account
            {
                AccountStatus = AccountStatus.Active,
                AccountType = accountType,
                Nickname = nickname,
                AccountHolders = accountHolders
            };
        }
    }
}
