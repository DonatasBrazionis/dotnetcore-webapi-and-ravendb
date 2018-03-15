using System.Security.Cryptography;
using System.Text;
using dotnetcore_webapi_and_ravendb.Contracts;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class PasswordHasherProvider : IPasswordHasherProvider
    {
        /// <summary>
        /// Calculates and returns BCrypt salted password hash
        /// more info: https://github.com/neoKushan/BCrypt.Net-Core
        /// </summary>
        /// <param name="password">Password phrase</param>
        /// <param name="salt">Salt for a password</param>
        /// <returns>BCrypt hashed password</returns>
        public string CalculateHash(string password, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(password + salt);
        }

        /// <summary>
        /// Check password
        /// more info: https://github.com/neoKushan/BCrypt.Net-Core
        /// </summary>
        /// <param name="password">Password phrase</param>
        /// <param name="salt">Salt for a password</param>
        /// <param name="hash">Hashed password</param>
        /// <returns></returns>
        public bool CheckPassword(string password, string salt, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password + salt, hash);
        }

        public string GetMD5(string str)
        {
            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}
