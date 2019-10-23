using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace WebApplication1
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
            services.AddHttpClient();

            services.AddControllersWithViews(options =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAccessTokenManagement()
                .ConfigureBackchannelHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://localhost:44303/";
                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                //options.ResponseType = "code id_token";
                options.UsePkce = true;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("api1");
                options.Scope.Add("offline_access");
                //options.Events.OnRedirectToIdentityProvider = context =>
                //{
                //    // only modify requests to the authorization endpoint
                //    if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                //{
                //        // generate code_verifier
                //        var codeVerifier = CryptoRandom.CreateUniqueId(32);

                //        // store codeVerifier for later use
                //        context.Properties.Items.Add("code_verifier", codeVerifier);

                //        // create code_challenge
                //        string codeChallenge;
                //    using (var sha256 = SHA256.Create())
                //    {
                //        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                //        codeChallenge = Base64Url.Encode(challengeBytes);
                //    }

                //        // add code_challenge and code_challenge_method to request
                //        context.ProtocolMessage.Parameters.Add("code_challenge", codeChallenge);
                //        context.ProtocolMessage.Parameters.Add("code_challenge_method", "S256");
                //    }

                //    return Task.CompletedTask;
                //};

            });
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
