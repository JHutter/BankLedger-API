using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankLedgerAPI.Models;

/*
 * data context so that changes aren't lost in statelessness of the conrollers
 */
namespace BankLedgerAPI
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Token> JWTBlacklist { get; set; }

        //public DbSet<Account> Accounts { get; set; }

        //public DbSet<Transaction> Transactions { get; set; }


        // TODO: this is kludgy, take it out
        public bool UsernameExists(string username)
        {
            foreach (User user in Users)
            {
                if (user.Username.Equals(username)) { return true; }
            }
            return false;
        }

        // TODO: this is a stub
        public bool AuthTokenInBlacklist(string attemptToken)
        {
            return true;
        }
    }



}