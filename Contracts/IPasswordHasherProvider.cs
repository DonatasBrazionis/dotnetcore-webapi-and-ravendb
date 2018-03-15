namespace dotnetcore_webapi_and_ravendb.Contracts
{
    public interface IPasswordHasherProvider
    {
        string CalculateHash(string password, string salt);
        string GetMD5(string str);
    }
}
