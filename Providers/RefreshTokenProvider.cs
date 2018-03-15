using System;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Models;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class RefreshTokenProvider : IRefreshTokenProvider
    {
        public RefreshTokenProvider(IRavenDatabaseProvider ravenDatabaseProvider, IPasswordHasherProvider passwordHasherProvider)
        {
            RavenDatabaseProvider = ravenDatabaseProvider;
            PasswordHasherProvider = passwordHasherProvider;
        }
        protected IRavenDatabaseProvider RavenDatabaseProvider { get; set; }
        protected IPasswordHasherProvider PasswordHasherProvider { get; set; }

        public string GenerateId(string refreshToken)
        {
            var hashedToken = PasswordHasherProvider.GetMD5(refreshToken);
            return $"refresh-token/{hashedToken}";
        }

        public async Task CreateAsync(string userId, string refreshToken, string remoteIpAddress, string userAgent, TimeSpan refreshTokenLifetime)
        {
            var id = GenerateId(refreshToken);
            var entity = new RefreshTokenDetails
            {
                Id = id,
                UserId = userId,
                RefreshToken = refreshToken,
                DateCreated = DateTime.UtcNow,
                IpAddress = remoteIpAddress,
                UserAgent = userAgent,
                DateExpires = DateTime.UtcNow + refreshTokenLifetime
            };
            await RavenDatabaseProvider.CreateEntity(entity);
        }

    }
}
