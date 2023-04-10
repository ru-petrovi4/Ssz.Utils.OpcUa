using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ssz.DataAccessGrpc.ServerBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ssz.DataAccessGrpcServer_OpcUaClient;
using Microsoft.Extensions.Configuration;

namespace Ssz.DataAccessGrpcServer_OpcUaClient
{
    public class Startup
    {
        #region public functions

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                //options.MaxReceiveMessageSize = null;
                //options.MaxSendMessageSize = 4 * 1024 * 1024; // 4 MB
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<DataAccessService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        #endregion
    }
}


//private readonly ILogger<Startup> _logger;

//public Startup(ILogger<Startup> logger)
//{
//    _logger = logger;
//}