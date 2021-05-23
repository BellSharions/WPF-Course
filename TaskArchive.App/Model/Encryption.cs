using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaskArchive.App.Model
{
    class Encryption
    {
        public string GetSalt()
        {
            var provider = new RNGCryptoServiceProvider();
            var salt = new byte[32];
            provider.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public string Encrypt(string password, string salt)
        {
            var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(salt));
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
