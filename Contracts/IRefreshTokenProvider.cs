using System;
using System.Threading.Tasks;

namespace dotnetcore_webapi_and_ravendb.Contracts
{
    public interface IRefreshTokenProvider
    {
        string GenerateId(string refreshToken);
        Task CreateAsync(string userId, string refreshToken, string remoteIpAddress, string userAgent, TimeSpan refreshTokenLifetime);
    }
}
