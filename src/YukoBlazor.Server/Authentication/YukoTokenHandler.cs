using System;
using System.Linq;
using System.IO;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YukoBlazor.Server.Models;

namespace YukoBlazor.Server.Authentication
{
    public class YukoTokenHandler : AuthenticationHandler<YukoTokenOptions>
    {
        public new const string Scheme = "Yuko";

        public YukoTokenHandler(
            IOptionsMonitor<YukoTokenOptions> options,  ILoggerFactory logger, 
            UrlEncoder encoder, IDataProtectionProvider dataProtection, 
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers["Authorization"].ToArray();
            if (authorization.Length == 0)
            {
                if (!string.IsNullOrEmpty(Request.Query["usr"]) && !string.IsNullOrEmpty(Request.Query["pwd"]))
                {
                    authorization = new[] { $"Yuko {Request.Query["usr"]} {Request.Query["pwd"]}" };
                }
                else
                {
                    return AuthenticateResult.NoResult();
                }
            }

            var identity = authorization.First().Substring("Yuko ".Length).Trim().Split(' ');

            var config = JsonConvert.DeserializeObject<Config>(await File.ReadAllTextAsync("config.json"));
            if (config.Manage.User != identity[0] || config.Manage.Password != identity[1])
            {
                return AuthenticateResult.Fail("User name or password is incorrect");
            }

            var claimIdentity = new ClaimsIdentity(Scheme, ClaimTypes.Name, ClaimTypes.Role);
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, identity[0]));
            claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimIdentity), Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}
