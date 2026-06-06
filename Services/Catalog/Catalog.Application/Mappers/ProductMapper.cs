using AutoMapper;
using Catalog.Application.Queries;

namespace Catalog.Application.Mappers
{
    public static class ProductMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
            cfg.AddProfile<ProductMappingProfile>();
        });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => lazy.Value;
    }
}
