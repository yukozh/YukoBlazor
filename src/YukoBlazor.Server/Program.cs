using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

[assembly: InternalsVisibleTo("YukoBlazor.Server.Tests")]
[assembly: InternalsVisibleTo("YukoBlazor.Client.Tests")]

namespace YukoBlazor.Server
{
    public class Program
    {
        internal static string DbPath = "blog.db";

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .Build())
                .UseStartup<Startup>();

            if (args.Length > 0)
            {
                for (var i = 0; i < args.Length; i += 2)
                {
                    if (args[i] == "-url")
                    {
                        string url = null;
                        try
                        {
                            url = args[i + 1];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Did not found the argument value for -url");
                            throw;
                        }
                        builder.UseUrls(url);
                    }
                    else if (args[i] == "-db")
                    {
                        try
                        {
                            DbPath = args[i + 1];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Did not found the argument value for -db");
                            throw;
                        }
                    }
                }
            }

            return builder.Build();
        }
    }
}
