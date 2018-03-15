namespace dotnetcore_webapi_and_ravendb.Contracts
{
    public interface IRefreshTokenProvider
    {
        string GenerateId(string refreshToken);
    }
}
