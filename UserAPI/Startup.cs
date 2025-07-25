using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using UserAPI.Repositories;
using UserAPI.Services;

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
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Default"]));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseRouting();
            app.UseDeveloperExceptionPage();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}
