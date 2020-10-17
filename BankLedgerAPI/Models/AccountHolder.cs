using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Models
{
    public class AccountHolder
    {
        public int Id { get; set; }
        public User Holder;
        public AccountHolderType AccountHolderType;
    }
}
