using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DoaMais.Web.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var usuario = new ClaimsIdentity();

            return await Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(usuario)
            ));
        }
    }
}
