using System;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Models;

namespace dotnetcore_webapi_and_ravendb.Contracts
{
    public interface ILoginProvider
    {
        bool SupportsUserLockout { get; }
        int AccessFailedMaxCount { get; }
        TimeSpan LockoutTime { get; }

        string GenerateId(string uniqueId);

        void SetPassword(LoginDetails entity, string password);
        Task<LoginDetails> GetEntity(string uniqueId);
        Task ResetAccessFailedCountAsync(LoginDetails entity);
        Task<bool> IsLockedOutAsync(LoginDetails entity);
        bool IsPasswordCorrect(LoginDetails entity, string password);
        Task AccessFailedAsync(LoginDetails entity);
    }
}
