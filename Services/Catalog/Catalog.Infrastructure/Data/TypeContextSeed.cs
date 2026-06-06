using System.Text.Json;
using System.Threading.Tasks;
using Catalog.Core.Entities;
using MongoDB.Driver;
using System.IO;
using System.Collections.Generic;

namespace Catalog.Infrastructure
{
    public static class TypeContextSeed
    {
        public static async Task SeedData(IMongoCollection<ProductType> typeCollection)
        {
            bool checkTypes = typeCollection.CountDocuments(FilterDefinition<ProductType>.Empty) > 0;

            if (!checkTypes)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "types.json");

                if (!File.Exists(path))
                {
                    Console.WriteLine($"Seed file not found at path: {path}");
                    return;
                }

                var typesData = await File.ReadAllTextAsync(path);
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                if (types == null || types.Count == 0)
                {
                    Console.WriteLine("No types found in JSON file.");
                    return;
                }

                await typeCollection.InsertManyAsync(types);

                foreach (var item in types)
                {
                    Console.WriteLine($"Inserted type: {item.Name}");
                }
            }
            else
            {
                Console.WriteLine("Types already exist. Skipping seeding.");
            }
        }
    }
}
