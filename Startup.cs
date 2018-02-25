using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;

namespace dotnetcore_webapi_and_ravendb
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
            services.AddCors();
            services.AddMvc();

            // This will instantiate a communication channel between application and the RavenDB server instance.
            services.AddSingleton<IDocumentStore>(provider =>
            {
                // More info: 
                // https://ravendb.net/docs/article-page/4.0/csharp/client-api/creating-document-store
                // https://ravendb.net/docs/article-page/4.0/csharp/client-api/setting-up-authentication-and-authorization

                // Load certificate
                var clientCertificate = new X509Certificate2(@"D:\RavenDB\no-name.Cluster.Settings\admin.client.certificate.no-name.pfx");

                var store = new DocumentStore
                {
                    Certificate = clientCertificate,
                    Database = "temp1",
                    Urls = new[] { "https://a.no-name.ravendb.community/" },
                    Conventions =
                    {
                        IdentityPartsSeparator = "-"
                    }
                };
                store.Initialize();
                return store;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowCredentials();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
