using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using IdentityModel.Client;
using KnowledgeSpace.ViewModels.Contents;
using KnowledgeSpace.WebPortal.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace KnowledgeSpace.WebPortal
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
            services.AddHttpClient("BackendApi").ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                //if (environment == Environments.Development)
                //{
                //    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                //}
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                return handler;
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            //IdentityModelEventSource.ShowPII = true; //Add this line
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   options.Events = new CookieAuthenticationEvents
                   {
                       // this event is fired everytime the cookie has been validated by the cookie middleware,
                       // so basically during every authenticated request
                       // the decryption of the cookie has already happened so we have access to the user claims
                       // and cookie properties - expiration, etc..
                       OnValidatePrincipal = async x =>
                       {
                           // since our cookie lifetime is based on the access token one,
                           // check if we're more than halfway of the cookie lifetime
                           var now = DateTimeOffset.UtcNow;
                           var timeElapsed = now.Subtract(x.Properties.IssuedUtc.Value);
                           var timeRemaining = x.Properties.ExpiresUtc.Value.Subtract(now);

                           if (timeElapsed > timeRemaining)
                           {
                               var identity = (ClaimsIdentity)x.Principal.Identity;
                               var accessTokenClaim = identity.FindFirst("access_token");
                               var refreshTokenClaim = identity.FindFirst("refresh_token");

                               // if we have to refresh, grab the refresh token from the claims, and request
                               // new access token and refresh token
                               var refreshToken = refreshTokenClaim.Value;
                               var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                               {
                                   Address = Configuration["Authorization:AuthorityUrl"],
                                   ClientId = Configuration["Authorization:ClientId"],
                                   ClientSecret = Configuration["Authorization:ClientSecret"],
                                   RefreshToken = refreshToken
                               });

                               if (!response.IsError)
                               {
                                   // everything went right, remove old tokens and add new ones
                                   identity.RemoveClaim(accessTokenClaim);
                                   identity.RemoveClaim(refreshTokenClaim);

                                   identity.AddClaims(new[]
                                   {
                                        new Claim("access_token", response.AccessToken),
                                        new Claim("refresh_token", response.RefreshToken)
                                    });

                                   // indicate to the cookie middleware to renew the session cookie
                                   // the new lifetime will be the same as the old one, so the alignment
                                   // between cookie and access token is preserved
                                   x.ShouldRenew = true;
                               }
                           }
                       }
                   };
               })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = Configuration["Authorization:AuthorityUrl"];
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.ClientId = Configuration["Authorization:ClientId"];
                    options.ClientSecret = Configuration["Authorization:ClientSecret"];
                    options.ResponseType = "code";

                    options.SaveTokens = true;

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("api.knowledgespace");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                    options.Events = new OpenIdConnectEvents
                    {
                        // that event is called after the OIDC middleware received the auhorisation code,
                        // redeemed it for an access token and a refresh token,
                        // and validated the identity token
                        OnTokenValidated = x =>
                        {
                            // store both access and refresh token in the claims - hence in the cookie
                            var identity = (ClaimsIdentity)x.Principal.Identity;
                            identity.AddClaims(new[]
                            {
                                new Claim("access_token", x.TokenEndpointResponse.AccessToken),
                                new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
                            });

                            // so that we don't issue a session cookie but one with a fixed expiration
                            x.Properties.IsPersistent = true;

                            // align expiration of the cookie with expiration of the
                            // access token
                            var accessToken = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken);
                            x.Properties.ExpiresUtc = accessToken.ValidTo;

                            return Task.CompletedTask;
                        }
                    };
                });

            var builder = services.AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<KnowledgeBaseCreateRequestValidator>());

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == Environments.Development)
            {
                builder.AddRazorRuntimeCompilation();
            }

            //Declare DI containers
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<ICategoryApiClient, CategoryApiClient>();
            services.AddTransient<IKnowledgeBaseApiClient, KnowledgeBaseApiClient>();
            services.AddTransient<ILabelApiClient, LabelApiClient>();
            services.AddTransient<IUserApiClient, UserApiClient>();
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
                app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains().Preload());
                app.UseXContentTypeOptions();
                app.UseReferrerPolicy(opts => opts.NoReferrer());
                app.UseXXssProtection(options => options.EnabledWithBlockMode());
                app.UseXfo(options => options.Deny());
            }
            app.UseSession();

            app.UseHttpsRedirection();

            //app.UseCsp(opts => opts
            //        .BlockAllMixedContent()
            //        .StyleSources(s => s.Self())
            //        .StyleSources(s => s.UnsafeInline())
            //        .FontSources(s => s.Self())
            //        .FormActions(s => s.Self())
            //        .FrameAncestors(s => s.Self())
            //        .ImageSources(s => s.Self())
            //    .ScriptSources(s => s.UnsafeInline())
            //    );
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                RoutingBuilder(endpoints);
            });
        }

        private static void RoutingBuilder(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                                    name: "My KBs",
                                    pattern: "/my-kbs",
                                    new { controller = "Account", action = "MyKnowledgeBases" });

            endpoints.MapControllerRoute(
                    name: "New KB",
                    pattern: "/new-kb",
                    new { controller = "Account", action = "CreateNewKnowledgeBase" });

            endpoints.MapControllerRoute(
                    name: "Edit KB",
                    pattern: "/edit-kb/{id}",
                    new { controller = "Account", action = "EditKnowledgeBase" });

            endpoints.MapControllerRoute(
                    name: "List By Tag Id",
                    pattern: "/tag/{tagId}",
                    new { controller = "KnowledgeBase", action = "ListByTag" });

            endpoints.MapControllerRoute(
                    name: "Search KB",
                    pattern: "/search",
                    new { controller = "KnowledgeBase", action = "Search" });

            endpoints.MapControllerRoute(
                    name: "KnowledgeBaseDetails",
                    pattern: "/kb/{seoAlias}-{id}",
                    new { controller = "KnowledgeBase", action = "Details" });

            endpoints.MapControllerRoute(
                    name: "ListByCategoryId",
                    pattern: "/cat/{categoryAlias}-{id}",
                    new { controller = "KnowledgeBase", action = "ListByCategoryId" });

            endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}