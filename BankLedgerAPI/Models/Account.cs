using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Models
{
    public class Account
    {
        private List<User> AccountHolders;
        public AccountType AccountType;
        public AccountStatus AccountStatus;
        public String Nickname;
        private List<Transaction> Transactions;
    }
}
