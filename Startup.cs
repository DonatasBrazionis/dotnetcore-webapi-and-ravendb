using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Providers;
using FluentValidation.AspNetCore;
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
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc()
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIdConnectConstants.TokenTypes.Bearer;
            })
            .AddOAuthValidation()
            .AddOpenIdConnectServer(options =>
            {
                options.ProviderType = typeof(AuthorizationProvider);
                options.TokenEndpointPath = "/connect/token";
                options.AllowInsecureHttp = HostingEnvironment.IsDevelopment();
            });

            services.AddScoped<AuthorizationProvider>();
            services.AddScoped<IRavenDatabaseProvider, RavenDBProvider>();
            services.AddScoped<IPasswordHasherProvider, PasswordHasherProvider>();
            services.AddScoped<ILoginProvider, LoginProvider>();
            services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();

            // This will instantiate a communication channel between application and the RavenDB server instance.
            services.AddSingleton<IDocumentStore>(provider =>
            {
                var clientCertificatePath = @"{path_to_your_client_certificate_pfx_file}";
                var databaseName = "{database_name}";
                var databaseUrl = "{database_url}";

                // Load certificate
                var clientCertificate = new X509Certificate2(clientCertificatePath);

                var store = new DocumentStore
                {
                    Certificate = clientCertificate,
                    Database = databaseName,
                    Urls = new[] { databaseUrl },
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

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}");
            });
        }
    }
}
