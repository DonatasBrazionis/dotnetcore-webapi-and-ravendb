using System;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Models;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class LoginProvider : ILoginProvider
    {
        public LoginProvider(IPasswordHasherProvider passwordHasherProvider, IRavenDatabaseProvider ravenDatabaseProvider)
        {
            PasswordHasherProvider = passwordHasherProvider;
            RavenDatabaseProvider = ravenDatabaseProvider;
        }
        protected IPasswordHasherProvider PasswordHasherProvider { get; set; }
        protected IRavenDatabaseProvider RavenDatabaseProvider { get; set; }

        public bool SupportsUserLockout { get; private set; } = true;
        public string GenerateId(string uniqueId) => $"login/{uniqueId}";

        public void SetPassword(LoginDetails entity, string password)
        {
            var salt = Guid.NewGuid().ToString().Replace("-", "");
            var saltedSecretHash = PasswordHasherProvider.CalculateHash(password, salt);
            entity.PasswordHash = saltedSecretHash;
            entity.PasswordSalt = salt;
        }

        public async Task<LoginDetails> GetEntity(string uniqueId)
        {
            var loginProviderId = GenerateId(uniqueId);
            var entity = await RavenDatabaseProvider.GetEntity<LoginDetails>(loginProviderId);
            return entity;
        }
    }
}
