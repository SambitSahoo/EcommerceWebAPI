using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Specs;

namespace Catalog.Application.Queries
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Define your mappings here
            CreateMap<ProductBrand, BrandResponse>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<ProductType, TypesResponse>().ReverseMap();
            CreateMap<Product, CreateProductCommand>().ReverseMap();
            CreateMap<Pagination<Product>, Pagination<ProductResponse>>().ReverseMap();
        }
    }
}