
using Microsoft.EntityFrameworkCore;
using SGP.Application.Interfaces;
using SGP.Application.Services;
using SGP.Persistence;
using SGP.Persistence.Db;
using System;

namespace SGP.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.

            builder.Services.AddTransient<IProductRepository, ProductRepository>();
            builder.Services.AddTransient<IProductService, ProductService>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
