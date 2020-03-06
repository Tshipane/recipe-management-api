using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RecipeManagement.Api.Infrastructure.Models.Jwt;
using RecipeManagement.Api.Infrastructure.Routing;
using RecipeManagement.Api.Infrastructure.Services.Jwt;
using RecipeManagement.Infrastructure.Configurations;
using RecipeManagement.Infrastructure.Cryptography;
using RecipeManagement.Infrastructure.Database;
using RecipeManagement.Infrastructure.Services;

namespace RecipeManagement.Api
{
    public class Startup
    {
        private const string SecretKey = "A25CDFB9-D517-4111-A89F-AD90C519CF4B";

        private readonly SymmetricSecurityKey
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("AllowAllHeaders", policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            // jwt wire up
            IConfigurationSection jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            string issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
            string audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.Configure<AppSettings>(Configuration);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = _signingKey
                    };
                });

            services.AddControllers(options => options.Conventions.Add(new CamelCaseRoutingConvention()))
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    });

            var builder = new ContainerBuilder();
            builder.Populate(services);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<RecipeManagementContext>().AsSelf();
            builder.RegisterType<AppSettingsAccessor>().AsImplementedInterfaces();
            builder.RegisterType<UserService>().AsImplementedInterfaces();
            builder.RegisterType<CryptographyService>().AsImplementedInterfaces();
            builder.RegisterType<JwtFactory>().AsImplementedInterfaces();
            builder.RegisterType<RecipeService>().AsImplementedInterfaces();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAllHeaders");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}