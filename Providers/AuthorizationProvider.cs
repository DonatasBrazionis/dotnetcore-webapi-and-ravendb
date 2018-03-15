using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using dotnetcore_webapi_and_ravendb.Contracts;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class AuthorizationProvider : OpenIdConnectServerProvider
    {
        public AuthorizationProvider(IRavenDatabaseProvider ravenDatabaseProvider, IRefreshTokenProvider refreshTokenProvider)
        {
            RavenDatabaseProvider = ravenDatabaseProvider;
            RefreshTokenProvider = refreshTokenProvider;
        }
        protected IRavenDatabaseProvider RavenDatabaseProvider { get; set; }
        protected IRefreshTokenProvider RefreshTokenProvider { get; set; }

        // Validate the grant_type and the client application credentials
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            // Reject the token requests that don't use grant_type=password or grant_type=refresh_token.
            if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only grant_type=password or grant_type=refresh_token are accepted by this server.");

                return;
            }

            // Check if refresh-token exists in DB
            if (context.Request.IsRefreshTokenGrantType())
            {
                var id = RefreshTokenProvider.GenerateId(context.Request.RefreshToken);
                if (!await RavenDatabaseProvider.IsEntityExists(id))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidClient,
                        description: "Invalid client.");
                    return;
                }
            }

            // Since there's only one application and since it's a public client
            // (i.e a client that cannot keep its credentials private), call Skip()
            // to inform the server the request should be accepted without 
            // enforcing client authentication.
            context.Skip();
            return;
        }

    }
}
