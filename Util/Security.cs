using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Util
{
    class Security
    {
        public static string Encrypt(string password)
        {
            var provider = SHA512.Create();
            string salt = "ThisSaltIsUncr@ble@-2300&^%$#@!";
            byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(salt + password));
            var pass = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            return pass;
        }   
    }    
}