using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public List<AccountHolder> AccountHolders;
        public AccountType AccountType;
        public AccountStatus AccountStatus;
        public String Nickname;
        private List<Transaction> Transactions;
    }
}
