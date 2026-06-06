using System.Text.Json;
using Catalog.Core.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure
{
    public static class CatalogContextSeed
    {
        public static async Task SeedData(IMongoCollection<Product> productCollection)
        {
            bool checkProducts = productCollection.CountDocuments(FilterDefinition<Product>.Empty) > 0;

            if (!checkProducts)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "products.json");

                if (!File.Exists(path))
                {
                    Console.WriteLine($"Seed file not found at path: {path}");
                    return;
                }

                var productsData = await File.ReadAllTextAsync(path);
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products == null || products.Count == 0)
                {
                    Console.WriteLine("No products found in JSON file.");
                    return;
                }

                await productCollection.InsertManyAsync(products);

                foreach (var item in products)
                {
                    Console.WriteLine($"Inserted product: {item.Name}");
                }
            }
            else
            {
                Console.WriteLine("Products already exist. Skipping seeding.");
            }
        }
    }
}