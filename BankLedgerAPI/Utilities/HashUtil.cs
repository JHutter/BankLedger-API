using System;
using System.Security.Cryptography;
using System.Text;

namespace BankLedgerAPI.Utilities
{
    public static class HashUtil
    {
        private static SHA256CryptoServiceProvider cryptoProvider;
        
        /// <summary>
        /// hashed the supplied string (no salt)
        /// </summary>
        /// <param name="password"></param>
        /// <returns>byte array of hashed password</returns>
        public static byte[] HashPassword(string password)
        {
            if (cryptoProvider == null)
            {
                cryptoProvider = new SHA256CryptoServiceProvider();
            }
            
            var hash = cryptoProvider.ComputeHash(Encoding.ASCII.GetBytes(password));
            return hash;
        }

        /// <summary>
        /// hashed the supplied password with a salt
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Tuple<byte[], string> HashWithNewSalt(string password)
        {
            if (cryptoProvider == null)
            {
                cryptoProvider = new SHA256CryptoServiceProvider();
            }
            var salt = Guid.NewGuid().ToString();
            var hash = cryptoProvider.ComputeHash(Encoding.ASCII.GetBytes(password+salt));
            Tuple<byte[], string> hashAndSalt = new Tuple<byte[], string>(hash, salt);
            
            return hashAndSalt;
        }

        /// <summary>
        /// hashes the supplied password and salt together
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>byte array after hashing</returns>
        public static byte[] HashWithSuppliedSalt(string password, string salt)
        {
            if (cryptoProvider == null)
            {
                cryptoProvider = new SHA256CryptoServiceProvider();
            }
            
            var hash = cryptoProvider.ComputeHash(Encoding.ASCII.GetBytes(password + salt));

            return hash;
        }
    }
}
