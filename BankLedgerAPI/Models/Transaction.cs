using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Models
{
    public class Transaction
    {
        public DateTime TransactionDate;
        public Decimal Amount;
        public String Source;
        public String UserNotes;
    }
}
