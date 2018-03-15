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
    }
}
