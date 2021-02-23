using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GremlinWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<GremlinClient>(
                (serviceProvider) =>
                {
                    var gremlinServer = new GremlinServer(
                        hostname: "localhost",
                        port: 8182,
                        enableSsl: false,
                        username: null,
                        password: null
                    );

                    var connectionPoolSettings = new ConnectionPoolSettings
                    {
                        MaxInProcessPerConnection = 32,
                        PoolSize = 4,
                        ReconnectionAttempts = 4,
                        ReconnectionBaseDelay = TimeSpan.FromSeconds(1)
                    };

                    return new GremlinClient(
                        gremlinServer: gremlinServer,
                        connectionPoolSettings: connectionPoolSettings
                    );
                }
            );

            services.AddSingleton<GraphTraversalSource>(
                (serviceProvider) =>
                {
                    GremlinClient gremlinClient = serviceProvider.GetService<GremlinClient>();
                    var driverRemoteConnection = new DriverRemoteConnection(gremlinClient, "g");
                    return AnonymousTraversalSource.Traversal().WithRemote(driverRemoteConnection);
                }
            );

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
