using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.IRepository;
using Core.Helper;
using Core.Helper.ConfigurationSections;
using Core.IServices;
using Core.Services;
using DatingApp.Consts;
using DatingApp.Middleware;
using Infrastructure.DataContext;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace DatingApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<CloudinarySection>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddMemoryCache();
            builder.Services.AddOutputCache();

            builder.Services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
           
              sqlOptions =>
              {
                  sqlOptions.EnableRetryOnFailure(
                      maxRetryCount: 5,
                      maxRetryDelay: TimeSpan.FromSeconds(30),
                      errorNumbersToAdd: null);
              }
        
             
              ));


            
          

            builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();



            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddScoped<IAppUserService,AppUserService>();
            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
            builder.Services.AddScoped<IUserLikesService, UserLikeService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddSingleton<MessageChachManager>();


            builder.Services.AddCors((opt) =>
            {
                opt.AddPolicy("ngPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:4200").AllowCredentials();

                });
            });


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration["jwt:issuer"],
                    ValidAudience = builder.Configuration["jwt:audience"],
                    ValidateLifetime=true,
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"]))
                };
            });


            builder.Services.AddAuthorizationBuilder()
                .AddPolicy(AuthorizationPolicies.AdminPolicy, policy => policy.RequireRole(UserRoles.AdminRole))
                .AddPolicy(AuthorizationPolicies.AdminModeratorPolicy, policy => {
                    policy.RequireRole(UserRoles.AdminRole);
                    policy.RequireRole(UserRoles.ModeratorRole);
                    }
                  );
                


            var app = builder.Build();

            app.UseErrorHandling();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("ngPolicy");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOutputCache();
            app.MapControllers();


            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            try
            {
                var context = service.GetRequiredService<AppDbContext>();
                var _userManager = service.GetRequiredService<UserManager<AppUser>>();
                var _roleManager = service.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                await context.Database.MigrateAsync();
                await Seed.SeedUsers(_userManager, _roleManager);
            }
            catch (Exception ex)
            {
                var logger=service.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "an error accured  during migration");
            }

            app.Run();
        }
    }
}
