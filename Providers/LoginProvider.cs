using System;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Models;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class LoginProvider : ILoginProvider
    {
        public LoginProvider(IPasswordHasherProvider passwordHasherProvider)
        {
            PasswordHasherProvider = passwordHasherProvider;
        }
        protected IPasswordHasherProvider PasswordHasherProvider { get; set; }

        public string GenerateId(string uniqueId) => $"login/{uniqueId}";

        public void SetPassword(LoginDetails entity, string password)
        {
            var salt = Guid.NewGuid().ToString().Replace("-", "");
            var saltedSecretHash = PasswordHasherProvider.CalculateHash(password, salt);
            entity.PasswordHash = saltedSecretHash;
            entity.PasswordSalt = salt;
        }
    }
}
