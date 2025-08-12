using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text;
using UserAPI.Repositories;
using UserAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using EmailService;
using UserAPI.Helpers;
namespace UserAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)

        {

            services.Configure<EmailConfiguration>(Configuration.GetSection("EmailConfiguration"));
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Api", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var secret = Configuration.GetValue<string>("AppSettings:Token");
            var issuer = Configuration.GetValue<string>("AppSettings:Issuer");
            var audiences = Configuration.GetSection("AppSettings:Audience").Get<List<string>>();
            var key = Convert.FromBase64String(secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudiences = audiences,

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero,
                };

            });

            services.AddHttpContextAccessor();

            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Default"]));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenRepository, TokenRepository>();

            services.AddScoped<IJWTHelper, JWTHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();

            }
           
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
