using System;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Models;

namespace dotnetcore_webapi_and_ravendb.Contracts
{
    public interface ILoginProvider
    {
        string GenerateId(string uniqueId);
        void SetPassword(LoginDetails entity, string password);
    }
}
