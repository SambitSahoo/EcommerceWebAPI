using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetProductByBrandQuery : IRequest<IList<ProductResponse>>
    {
        public string BrandName { get; }

        public GetProductByBrandQuery(string brandName)
        {
            BrandName = brandName;
        }
    }
}