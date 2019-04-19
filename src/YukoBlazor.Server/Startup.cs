using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using YukoBlazor.Server.Authentication;
using YukoBlazor.Server.Models;

namespace YukoBlazor.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddDbContext<BlogContext>(x => x.UseSqlite($"Data source={Program.DbPath}"));
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddAuthentication(x => x.DefaultScheme = YukoTokenHandler.Scheme)
                .AddYukoToken();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            app.UseErrorHandlingMiddleware();
            //app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseBlazorDebugging();
            }

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseBlazor<Client.Startup>();

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                await SampleData.InitializeYuukoBlog(serviceScope.ServiceProvider);
            }
        }
    }
}
