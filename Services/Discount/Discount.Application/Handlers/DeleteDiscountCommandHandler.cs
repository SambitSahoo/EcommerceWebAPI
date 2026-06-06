using Discount.Application.Commands;
using Discount.Core.IRepo;
using MediatR;

namespace Discount.Application.Handlers
{ 
    public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, bool>
    {
        private readonly IDiscountRepo _repository; 
        public DeleteDiscountCommandHandler(IDiscountRepo repository)
        {
            _repository = repository; 
        }
        public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);
            return deleted;
        }
    }
}