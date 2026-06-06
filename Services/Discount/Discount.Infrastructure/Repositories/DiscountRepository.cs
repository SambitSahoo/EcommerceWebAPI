using Dapper;
using Discount.Core.Entities;
using Discount.Core.IRepo;
using Microsoft.Extensions.Configuration;

namespace Discount.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepo
    {
        private readonly IConfiguration _configuration;
        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Coupon> GetDiscount(string productName)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });
            if (coupon == null)
            {
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Available" };
            }
            return coupon;
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount, ExpiryDate, IsActive) VALUES (@ProductName, @Description, @Amount, @ExpiryDate, @IsActive)",
                                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, ExpiryDate = coupon.ExpiryDate, IsActive = coupon.IsActive });
            if (affected == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount, ExpiryDate=@ExpiryDate,IsActive=@IsActive WHERE Id = @Id",
                                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, coupon.ExpiryDate, coupon.IsActive, Id = coupon.Id });
            if (affected == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> DeleteDiscount(string productName)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                                            new { ProductName = productName });
            if (affected == 0)
            {
                return false;
            }
            return true;
        }        
    }
}