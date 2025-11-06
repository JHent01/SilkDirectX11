using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace SilkDirectX11.SignalR
{
    public static class SignalRHost
    {
        private static IHost? _host;
        private static readonly object _gate = new();

        public static IHost StartIfNeeded(int port = 5178)
        {
            if (_host != null)
                return _host;

            lock (_gate)
            {
                if (_host != null)
                    return _host;

                _host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(web =>
                    {
                        web.UseKestrel(opts =>
                        {
                            opts.AddServerHeader = false;
                        });
                        web.UseUrls($"http://localhost:{port}");
                        web.ConfigureServices(services =>
                        {
                            services.AddSignalR();
                        });
                        web.Configure(app =>
                        {
                            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
                            if (env.IsDevelopment())
                            {
                                app.UseDeveloperExceptionPage();
                            }

                            app.UseRouting();
                            app.UseEndpoints(MapEndpoints);
                        });
                    })
                    .Build();

                
                new Thread(() => _host!.Run()) { IsBackground = true, Name = "SignalRHost" }.Start();
                return _host!;
            }
        }

        private static void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<PointsHub>("/hubs/points");
        }
    }
}
