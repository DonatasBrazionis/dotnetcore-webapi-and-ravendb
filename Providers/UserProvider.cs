using System;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Models;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class UserProvider : IUserProvider
    {
        public UserProvider(IRavenDatabaseProvider ravenDatabaseProvider)
        {
            RavenDatabaseProvider = ravenDatabaseProvider;
        }
        protected IRavenDatabaseProvider RavenDatabaseProvider { get; set; }

        public async Task<bool> IsBannedAsync(User user)
        {
            if (user.DateBanEndsUtc > DateTime.UtcNow)
            {
                return true;
            }
            // Remove ban if time ended
            if (user.DateBanEndsUtc <= DateTime.UtcNow)
            {
                await RemoveBanAsync(user);
            }
            return false;
        }

        private async Task RemoveBanAsync(User user)
        {
            user.DateBanEndsUtc = null;
            user.BanDescription = null;
            user.BannedBy = null;
            await RavenDatabaseProvider.UpdateEntity(user.Id, user);
        }
    }
}
