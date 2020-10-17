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
        public DbSet<AccountHolder> AccountHolders { get; set; }
        public DbSet<Account> Accounts { get; set; }


        // TODO: this is a stub
        public bool AuthTokenInBlacklist(string attemptToken)
        {
            return true;
        }
    }



}