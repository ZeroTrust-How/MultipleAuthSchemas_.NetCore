using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace MultiAuthSchemas
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer("AzureAd", options =>
                    {
                        options.Audience = Configuration["AzureAd:ClientId"];
                        options.Authority = $"{Configuration["AzureAd:Instance"]}{Configuration["AzureAd:TenantId"]}";
                    }
                    )
                    .AddMicrosoftIdentityWebApi(options =>
                    {
                        Configuration.Bind("AzureAdB2C", options);

                        options.TokenValidationParameters.NameClaimType = "name";
                    },
                        options => { Configuration.Bind("AzureAdB2C", options); });


            services.Configure<AzureAdB2COptions>(Configuration.GetSection(Constants.AzureAdB2C));

            services.AddScoped<IGraphService, GraphService>();

            services.AddControllers();

            services.AddAuthorization(options =>
            {
                //[Authorize] - When this header is used in controller, both AAD/AADB2C token accepted.
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("AzureAd", "Bearer")
                    .Build();
                    
                //[Authorize(Policy = "AADAuth")] - When this header used in controller, only AAD token accepted.
                options.AddPolicy("AADAuth", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("AzureAd")
                    //.RequireClaim("role", "admin.readwrite") //could be role/scope
                    .Build());

            });

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
