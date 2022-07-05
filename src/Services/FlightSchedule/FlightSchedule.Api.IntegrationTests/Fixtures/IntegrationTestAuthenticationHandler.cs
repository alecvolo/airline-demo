using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlightSchedule.Api.IntegrationTests.Fixtures
{
    internal class IntegrationTestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public IntegrationTestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, "IntegrationTest User"),
                new Claim(ClaimTypes.NameIdentifier, "IntegrationTest User"),
                new Claim("a-custom-claim", "squirrel 🐿️"),
                new Claim("http://schemas.microsoft.com/identity/claims/scope", "Read Write")
            };
            var identity = new ClaimsIdentity(claims, "IntegrationTest");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "IntegrationTest");
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}
