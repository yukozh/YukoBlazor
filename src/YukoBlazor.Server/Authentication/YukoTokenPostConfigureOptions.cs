﻿using Microsoft.Extensions.Options;

namespace YukoBlazor.Server.Authentication
{
    /// <summary>
    /// Used to setup defaults for all <see cref="JwtBearerOptions"/>.
    /// </summary>
    public class YukoTokenPostConfigureOptions : IPostConfigureOptions<YukoTokenOptions>
    {
        /// <summary>
        /// Invoked to post configure a JwtBearerOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, YukoTokenOptions options)
        {
        }
    }
}