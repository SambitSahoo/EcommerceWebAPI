using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discount.Infrastructure.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Discount database migration started");
                    ApplyMigrations(configuration);
                    logger.LogInformation("Discount database migration completed");
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database.");
                    throw;

                }
            }

            return host;
        }

        private static void ApplyMigrations(IConfiguration configuration)
        {
            using var connection = new Npgsql.NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            connection.Open();

            using var command = new Npgsql.NpgsqlCommand
            {
                Connection = connection
            };

            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, ProductName VARCHAR(500) NOT NULL, Description TEXT, Amount INT, ExpiryDate TIMESTAMP, IsActive BOOLEAN)";
            command.ExecuteNonQuery();

            // Yonex Rackets
            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Yonex Astrox 100 ZZ', 'Premium Yonex racket discount', 250, '2025-12-31', true);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Yonex Nanoflare 800', 'Speed-focused Yonex racket offer', 220, '2025-11-30', true);";
            command.ExecuteNonQuery();

            // Li-Ning Rackets
            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Li-Ning Turbo X 90', 'Li-Ning racket promotion', 180, '2025-10-15', true);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Li-Ning Windstorm 72', 'Lightweight Li-Ning racket deal', 160, '2025-09-30', true);";
            command.ExecuteNonQuery();

            // Victor Rackets
            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Victor Thruster K Falcon', 'Powerful Victor racket discount', 200, '2026-01-15', true);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Victor Jetspeed S 12', 'Victor speed racket offer', 190, '2025-12-15', true);";
            command.ExecuteNonQuery();

            // Adidas Shoes
            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Adidas Adizero FastCourt', 'Adidas badminton shoe discount', 120, '2025-10-31', true);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Adidas Stabil Next Gen', 'Adidas indoor shoe deal', 140, '2025-11-30', true);";
            command.ExecuteNonQuery();

            // Reebok Shoes
            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Reebok Court Blaze', 'Reebok court shoe offer', 100, '2025-09-30', true);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Reebok ZQuick Dash', 'Reebok training shoe discount', 110, '2025-12-01', true);";
            command.ExecuteNonQuery();

            // Nike Shoes
            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Nike Zoom HyperAce 2', 'Nike indoor shoe deal', 150, '2026-02-28', true);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) " +
                                  "VALUES('Nike Air Zoom GP Turbo', 'Nike court shoe promotion', 170, '2025-12-20', true);";
            command.ExecuteNonQuery();
        }
    }
}