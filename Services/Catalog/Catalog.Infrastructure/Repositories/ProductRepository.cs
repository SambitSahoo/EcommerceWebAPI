using System;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypesRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            return await _context.Products.Find(filter).FirstOrDefaultAsync();
        }
        async Task<Pagination<Product>> IProductRepository.GetProducts(CatalogSpecParams specParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(specParams.Search))
            {
                filter = filter & builder.Where(P => P.Name.ToLower().Contains(specParams.Search.ToLower()));
            }
            if (!string.IsNullOrEmpty(specParams.BrandId))
            {
                var brandFilter = builder.Eq(p => p.Brands.Id, specParams.BrandId);
                filter = filter & brandFilter;
            }
            if (!string.IsNullOrEmpty(specParams.TypeId))
            {
                var typeFilter = builder.Eq(p => p.Types.Id, specParams.TypeId);
                filter = filter & typeFilter;
            }
            var totalItems = await _context.Products.CountDocumentsAsync(filter);
            var data = await DataFilter(specParams, filter);
            return new Pagination<Product>(specParams.PageIndex, specParams.PageSize, (int)totalItems, data);
        }
        async Task<IEnumerable<Product>> IProductRepository.GetProductsByBrand(string brandName)
        {
            var filter = Builders<Product>.Filter.Regex("Brands.Name", new BsonRegularExpression(brandName, "i"));
            return await _context.Products.Find(filter).ToListAsync();
        }
        async Task<IEnumerable<Product>> IProductRepository.GetProductsByName(string name)
        {
            return await _context.Products.Find(p => p.Name.ToLower() == name.ToLower()).ToListAsync();
        }
        async Task<Product> IProductRepository.CreateProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = Guid.NewGuid().ToString();
            }
            await _context.Products.InsertOneAsync(product);
            return product;
        }
        public async Task<bool> DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq("_id", id);
            var deletedProduct = await _context.Products.DeleteOneAsync(filter);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }
        async Task<bool> IProductRepository.UpdateProduct(Product product)
        {
            var updatedProduct = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }
        async Task<List<ProductBrand>> IBrandRepository.GetAllBrandsAsync()
        {
            return await _context.Brands.Find(_ => true).ToListAsync();

        }
        async Task<IEnumerable<ProductType>> ITypesRepository.GetAllTypes()
        {
            return await _context.Types.Find(t => true).ToListAsync();
        }
        private async Task<IReadOnlyList<Product>> DataFilter(CatalogSpecParams specParams, FilterDefinition<Product> filter)
        {
            var sort = Builders<Product>.Sort.Ascending(p => p.Name);
            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc":
                        sort = Builders<Product>.Sort.Ascending(p => p.Price);
                        break;
                    case "priceDesc":
                        sort = Builders<Product>.Sort.Descending(p => p.Price);
                        break;
                    default:
                        sort = Builders<Product>.Sort.Ascending(p => p.Name);
                        break;
                }
            }
            return await _context.Products.Find(filter)
                .Sort(sort)
                .Skip(specParams.PageSize * (specParams.PageIndex - 1))
                .Limit(specParams.PageSize)
                .ToListAsync();
        }

    }
}