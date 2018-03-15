using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Models;
using Microsoft.AspNetCore.Authentication;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    /// <summary>
    /// Resource owner password credentials (ROPC) flow implementation using AspNet.Security.OpenIdConnect.Server (ASOS)
    /// </summary>
    public class AuthorizationProvider : OpenIdConnectServerProvider
    {
        public AuthorizationProvider(
            IRavenDatabaseProvider ravenDatabaseProvider,
            IRefreshTokenProvider refreshTokenProvider, ILoginProvider loginProvider, IUserProvider userProvider
        )
        {
            RavenDatabaseProvider = ravenDatabaseProvider;
            RefreshTokenProvider = refreshTokenProvider;
            LoginProvider = loginProvider;
            UserProvider = userProvider;
        }
        protected IRavenDatabaseProvider RavenDatabaseProvider { get; set; }
        protected IRefreshTokenProvider RefreshTokenProvider { get; set; }
        protected ILoginProvider LoginProvider { get; set; }
        protected IUserProvider UserProvider { get; set; }

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

        // Implementing HandleTokenRequest to issue an authentication ticket containing the user claims
        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            // Only handle grant_type=password requests and let ASOS
            // process grant_type=refresh_token requests automatically.
            if (context.Request.IsPasswordGrantType())
            {
                // Get user login data.
                var loginDetails = await LoginProvider.GetEntity(context.Request.Username);
                if (loginDetails == null)
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }
                if (loginDetails.UniqueId != context.Request.Username)
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }

                // Get user data
                var user = await RavenDatabaseProvider.GetEntity<User>(loginDetails.UserId);
                if (user == null)
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid username or password.");
                    return;
                }
                // Ensure the user is allowed to sign in.
                if (await UserProvider.IsBannedAsync(user))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "The specified user is not allowed to sign in.");
                    return;
                }

                // Ensure the user is not already locked out.
                if (LoginProvider.SupportsUserLockout && await LoginProvider.IsLockedOutAsync(loginDetails))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }

                // Ensure the password is valid.
                if (!LoginProvider.IsPasswordCorrect(loginDetails, context.Request.Password))
                {
                    if (LoginProvider.SupportsUserLockout)
                    {
                        // Increment lock out count
                        await LoginProvider.AccessFailedAsync(loginDetails);
                    }
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }
                if (LoginProvider.SupportsUserLockout)
                {
                    // Reset lock out data
                    await LoginProvider.ResetAccessFailedCountAsync(loginDetails);
                }

                var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);

                // Note: the subject claim is always included in both identity and
                // access tokens, even if an explicit destination is not specified.
                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, loginDetails.UserId);

                // Add user-id
                identity.AddClaim(ClaimTypes.Name, user.Id,
                    OpenIdConnectConstants.Destinations.AccessToken);

                // Add user-role
                identity.AddClaim(ClaimTypes.Role, user.Type.ToString(),
                    OpenIdConnectConstants.Destinations.AccessToken);

                // Create a new authentication ticket holding the user identity.
                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties(),
                    OpenIdConnectServerDefaults.AuthenticationScheme);

                // Set the list of scopes granted to the client application.
                // (specify offline_access to issue a refresh token).
                ticket.SetScopes(OpenIdConnectConstants.Scopes.OfflineAccess);

                context.Validate(ticket);
            }

            return;
        }

        // Save refresh-token
        public override async Task ApplyTokenResponse(ApplyTokenResponseContext context)
        {
            if (context.Response.Error == null && context.Response.RefreshToken != null)
            {
                if (context.Request.IsRefreshTokenGrantType())
                {
                    var refreshTokenId = RefreshTokenProvider.GenerateId(context.Request.RefreshToken);
                    await RavenDatabaseProvider.DeleteEntity(refreshTokenId);
                }

                string remoteIpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                string userAgent = null;
                if (context.HttpContext.Request.Headers.ContainsKey("User-Agent"))
                {
                    userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
                }
                await RefreshTokenProvider.CreateAsync(
                    context.Ticket.Principal.Identity.Name,
                    context.Response.RefreshToken,
                    remoteIpAddress,
                    userAgent,
                    context.Options.RefreshTokenLifetime);
            }
            return;
        }

    }
}
