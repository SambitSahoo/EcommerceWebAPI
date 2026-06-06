using AutoMapper;
using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, List<BrandResponse>>
    {
        private readonly IBrandRepository _brandRepository;

        public GetAllBrandsHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<List<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _brandRepository.GetAllBrandsAsync();
            return brands.Select(b => new BrandResponse { Id = b.Id.ToString(), Name = b.Name }).ToList();
        }
    }

}