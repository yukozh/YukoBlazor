using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using YukoBlazor.Server.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class YukoTokenExtensions
    {
        public static AuthenticationBuilder AddYukoToken(
            this AuthenticationBuilder builder)
            => builder.AddYukoToken(YukoTokenHandler.Scheme, null, _ => { });

        public static AuthenticationBuilder AddYukoToken(
            this AuthenticationBuilder builder, 
            string authenticationScheme, 
            string displayName,
            Action<YukoTokenOptions> configureOptions)
        {
            builder
                .Services
                .TryAddEnumerable(
                    ServiceDescriptor.Singleton<IPostConfigureOptions<YukoTokenOptions>, YukoTokenPostConfigureOptions>());

            return builder.AddScheme<YukoTokenOptions, YukoTokenHandler>(
                authenticationScheme, 
                displayName, 
                configureOptions);
        }
    }
}