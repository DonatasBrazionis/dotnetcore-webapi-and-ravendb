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
        public int AccessFailedMaxCount { get; private set; } = 3;
        public TimeSpan LockoutTime { get; private set; } = new TimeSpan(0, 5, 0);

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

        public async Task ResetAccessFailedCountAsync(LoginDetails entity)
        {
            entity.DateLockoutEndsUtc = null;
            entity.AccessFailedCount = 0;

            await RavenDatabaseProvider.UpdateEntity(entity.Id, entity);
        }

        public async Task<bool> IsLockedOutAsync(LoginDetails entity)
        {
            if (entity.DateLockoutEndsUtc == null)
            {
                return false;
            }
            if (entity.DateLockoutEndsUtc > DateTime.UtcNow)
            {
                return true;
            }
            // Remove lock-out if time ended
            if (entity.DateLockoutEndsUtc <= DateTime.UtcNow)
            {
                await ResetAccessFailedCountAsync(entity);
            }
            return false;
        }

        public bool IsPasswordCorrect(LoginDetails entity, string password)
        {
            var correctPassword = PasswordHasherProvider.CheckPassword(password, entity.PasswordSalt, entity.PasswordHash);
            if (correctPassword)
            {
                return true;
            }
            return false;
        }

        public async Task AccessFailedAsync(LoginDetails entity)
        {
            entity.AccessFailedCount++;
            if (entity.AccessFailedCount >= AccessFailedMaxCount)
            {
                entity.DateLockoutEndsUtc = DateTime.UtcNow.Add(LockoutTime);
            }
            await RavenDatabaseProvider.UpdateEntity(entity.Id, entity);
        }

    }
}
