using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Models;

namespace dotnetcore_webapi_and_ravendb.Contracts
{
    public interface IUserProvider
    {
        Task<bool> IsBannedAsync(User user);
    }
}
