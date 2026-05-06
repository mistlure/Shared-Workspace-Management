using API.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Try to get the connection string from configuration, if not found throw an exception
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("String connection is not found.");

            // Register the connection factory as a singleton service
            builder.Services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(connectionString));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            builder.Services.AddScoped<IWorkplaceRepository, WorkplaceRepository>();
            builder.Services.AddScoped<IOccupancyRepository, OccupancyRepository>();
            builder.Services.AddScoped<IStatusHistoryRepository, StatusHistoryRepository>();

            builder.Services.AddScoped<IOccupancyService, OccupancyService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
                DbInitializer.Initialize(factory);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
