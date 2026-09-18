using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WebApplication1.Security
{
    public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
        {
            _configuration = configuration;
        }
        protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(
            "Authorization",
            out var authorizationHeader))
            {
                return Task.FromResult(
                AuthenticateResult.NoResult());
            }
            if (!AuthenticationHeaderValue.TryParse(
            authorizationHeader.ToString(),
            out AuthenticationHeaderValue? header) ||
            !string.Equals(
            header.Scheme,
           "Basic",
           StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(header.Parameter))
            {
                return Task.FromResult(
                AuthenticateResult.Fail(
                "Invalid Authorization header."));
            }
            string decodedCredentials;
            try
            {
                byte[] credentialBytes =
                Convert.FromBase64String(
                header.Parameter);
                decodedCredentials =
                Encoding.UTF8.GetString(
                credentialBytes);
            }
            catch (FormatException)
            {
                return Task.FromResult(
                AuthenticateResult.Fail(
                "Invalid Basic authentication encoding."));
            }
            string[] credentials =
            decodedCredentials.Split(
            ':',
           2);
            if (credentials.Length != 2)
            {
                return Task.FromResult(
                AuthenticateResult.Fail(
                "Invalid Basic authentication credentials."));
            }
            string username =
            credentials[0];
            string password =
            credentials[1];
            string? role = null;
            string? permission = null;
            string? adminUsername =
            _configuration[
            "BasicAuth:AdminUsername"];
            string? adminPassword =
            _configuration[
            "BasicAuth:AdminPassword"];
            string? editorUsername =
            _configuration[
            "BasicAuth:EditorUsername"];
            string? editorPassword =
            _configuration[
            "BasicAuth:EditorPassword"];
            if (username == adminUsername &&
            password == adminPassword)
            {
                role = "Admin";
                permission = "ManageBlog";
            }
            else if (
            username == editorUsername &&
            password == editorPassword)
            {
                role = "Editor";
                permission = "EditBlog";
            }
            else
            {
                return Task.FromResult(
                AuthenticateResult.Fail(
                "Invalid username or password."));
            }
            Claim[] claims =
            [
            new Claim(
             ClaimTypes.NameIdentifier,
            username),
             new Claim(
             ClaimTypes.Name,
            username),
             new Claim(
             ClaimTypes.Role,
            role),
             new Claim(
             "Permission",
             permission)
            ];
            ClaimsIdentity identity =
            new(
            claims,
           Scheme.Name);
            ClaimsPrincipal principal =
            new(identity);
            AuthenticationTicket ticket =
            new(
            principal,
           Scheme.Name);
            return Task.FromResult(
            AuthenticateResult.Success(
            ticket));
        }
        protected override Task HandleChallengeAsync(
        AuthenticationProperties properties)
        {
            Response.Headers.WWWAuthenticate =
            "Basic realm=\"BlogApi\"";
            Response.StatusCode =
            StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

    }
}
