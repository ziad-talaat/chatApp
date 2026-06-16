using Core.Domain.Entities;
using Core.Domain.IRepository;
using Core.IServices;
using Core.Services;
using Infrastructure.DataContext;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace DatingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            //builder.Services.AddIdentityCore<AppUser>()
            //    .AddRoles<IdentityRole<Guid>>()
            //    .AddSignInManager()
            //    .AddDefaultTokenProviders()
            //    .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();



            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddScoped<IAppUserService,AppUserService>();
            builder.Services.AddCors((opt) =>
            {
                opt.AddPolicy("ngPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:4200");

                });
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("ngPolicy"); 

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
