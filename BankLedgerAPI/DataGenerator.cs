using BankLedgerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankLedgerAPI
{
    /*
     * Data generator
     * Seeds the data context with sample data
     * For testing, can be cut out for prod
     * */
    public class DataGenerator
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            //using (var context = new DataContext(
            //    serviceProvider.GetRequiredService<DbContextOptions<DataContext>>()))
            //{

            //    if (context.Users.Any())
            //    {
            //        return;
            //    }

            //    using (var sha1 = new SHA1CryptoServiceProvider())
            //    {
            //        var sha1data = sha1.ComputeHash(Encoding.ASCII.GetBytes("testpass"));
            //        context.Users.Add(new User {
            //            FName = "Leia",
            //            LName = "Organa",
            //            Username = "leia2",
            //            Password = sha1data});
            //    }

            //    context.SaveChanges();
            //}
        }
    }
}