using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class DeleteProductByIdCommandHandler : IRequestHandler<DeleteProductByIdCommand, DeleteResponse>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductByIdCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<DeleteResponse> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _productRepository.DeleteProduct(request.Id);

            if (!deleted)
                return new DeleteResponse(false, $"Product with ID {request.Id} not found or could not be deleted.");

            return new DeleteResponse(true, $"Product with ID {request.Id} deleted successfully.");
        }
    }
}
    