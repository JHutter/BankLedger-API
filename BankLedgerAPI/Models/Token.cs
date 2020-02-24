using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Models
{
    public class Token
    {
        public int Id { get; set; }
        public DateTime Expiration { get; set; }
        public string TokenString { get; set; }
        [NotMapped]
        public SecurityToken JWTToken { get; set; }
    }
}
