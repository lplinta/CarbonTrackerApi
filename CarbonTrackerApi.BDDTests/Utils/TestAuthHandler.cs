using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarbonTrackerApi.BDDTests.Utils;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public IList<Claim> Claims { get; } = [];
}

public class TestAuthHandler(
    IOptionsMonitor<TestAuthHandlerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<TestAuthHandlerOptions>(options, logger, encoder)
{
    public const string AuthenticationScheme = "TestScheme";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(Options.Claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}