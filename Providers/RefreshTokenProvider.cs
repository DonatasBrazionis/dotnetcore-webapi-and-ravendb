using dotnetcore_webapi_and_ravendb.Contracts;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class RefreshTokenProvider : IRefreshTokenProvider
    {
        public RefreshTokenProvider(IPasswordHasherProvider passwordHasherProvider)
        {
            PasswordHasherProvider = passwordHasherProvider;
        }
        protected IPasswordHasherProvider PasswordHasherProvider { get; set; }

        public string GenerateId(string refreshToken)
        {
            var hashedToken = PasswordHasherProvider.GetMD5(refreshToken);
            return $"refresh-token/{hashedToken}";
        }
    }
}
