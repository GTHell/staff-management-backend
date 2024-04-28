using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using StaffManagement.Models;
using Microsoft.AspNetCore.Hosting;

namespace Tests.IntegrationTests.Helpers;

public class CustomWebApplicationFactory<TProgram>
: WebApplicationFactory<TProgram> where TProgram : class

{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StaffDb>));

            // services.Remove(descriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            // services.AddSingleton<StaffDb>(container =>
            // {
            //     var connection = new SqliteConnection("DataSource=:memory:");
            //     connection.Open();

            //     return connection;
            // });


            // Replace the default DbContext registration with SQLite in-memory database
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            services.AddDbContext<StaffDb>(options =>
            {
                options.UseSqlite("DataSource=:memory:")
                       .UseInternalServiceProvider(serviceProvider);
            });

            // Create a new scope to obtain a reference to the database context
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StaffDb>();

                // Ensure the in-memory database is Deleted and Created
                // Otherwise, the database will be re-used and data will be stale
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Seed the database with test data
                SeedSampleData(db);
            }
        });
    }

    private void SeedSampleData(StaffDb db)
    {
        // Add sample data to the database
        db.Staffs.AddRange(
            new Staff { StaffId = "007", Fullname = "John Bond", Birthdate = new DateTime(1990, 1, 1), Gender = 1 },
            new Staff { StaffId = "008", Fullname = "Jane Bond", Birthdate = new DateTime(1995, 5, 10), Gender = 2 }
        );
        db.SaveChanges();
    }
}