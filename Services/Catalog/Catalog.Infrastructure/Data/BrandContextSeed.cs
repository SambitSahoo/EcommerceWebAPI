using System.Text.Json;
using System.Threading.Tasks;
using Catalog.Core.Entities;
using MongoDB.Driver;
using System.IO; // Ensure this is included
using System.Collections.Generic; // Ensure this is included

namespace Catalog.Infrastructure
{
    public static class BrandContextSeed
    {
        public static async Task SeedData(IMongoCollection<ProductBrand> brandCollection)
        {
            // Check if any brands already exist
            bool checkBrands = brandCollection.CountDocuments(FilterDefinition<ProductBrand>.Empty) > 0;

            if (!checkBrands)
            {
                // Build the correct path to brands.json
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "brands.json");

                if (!File.Exists(path))
                {
                    Console.WriteLine($"Seed file not found at path: {path}");
                    return;
                }

                // Read and deserialize the JSON file
                var brandsData = await File.ReadAllTextAsync(path);
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                if (brands == null || brands.Count == 0)
                {
                    Console.WriteLine("No brands found in JSON file.");
                    return;
                }

                Console.WriteLine($"Seeding {brands.Count} brands...");

                // Insert all brands into MongoDB
                await brandCollection.InsertManyAsync(brands);

                foreach (var item in brands)
                {
                    Console.WriteLine($"Inserted brand: {item.Name}");
                }
            }
            else
            {
                Console.WriteLine("Brands already exist. Skipping seeding.");
            }
        }
    }
}
