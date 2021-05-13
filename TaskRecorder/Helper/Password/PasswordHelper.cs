using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRecorder.Helper.Password
{
    public class PasswordHelper
    {
        public static string key = "ThisIsMyOwnP@$$w0rdHelPer*";
        public static string Encrypt(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return "";
            password += key;
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passwordBytes);
        }
        public static string Decrypt(string encodedData)
        {
            if (string.IsNullOrWhiteSpace(encodedData)) return "";
            var encodedBytes = Convert.FromBase64String(encodedData);
            var result = Encoding.UTF8.GetString(encodedBytes);
            result = result.Substring(0, result.Length - key.Length);
            return result;
        }
    }
}
