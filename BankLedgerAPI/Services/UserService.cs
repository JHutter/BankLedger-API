using BankLedgerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace BankLedgerAPI.Services
{
    // interfact to make auth easier. adapted from auth filter sample
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }
        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.Password))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }






        private static bool VerifyPasswordHash(string password, byte[] storedPass)
        {
            //var sha1 = new SHA1CryptoServiceProvider();  // will garbage collection dispose this?


            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var sha1data = sha1.ComputeHash(Encoding.ASCII.GetBytes(password));
                string hashedPasswordToString = Convert.ToBase64String(sha1data);
                return (hashedPasswordToString.Equals(Convert.ToBase64String(storedPass)));
            }

            //return false;
        }


        // TODO implement these
        public User Create(User user, string password)
        {
            throw new NotImplementedException();
        }

        public void Update(User user, string password = null)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}