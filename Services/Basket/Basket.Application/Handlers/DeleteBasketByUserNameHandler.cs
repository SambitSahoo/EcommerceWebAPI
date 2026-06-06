using System.Threading;
using System.Threading.Tasks;
using Basket.Application.Commands;
using Basket.Core.Repo;
using MediatR;

namespace Basket.Application.Handlers
{
    public class DeleteBasketByUserNameHandler : IRequestHandler<DeleteBasketByUserNameCommand, Unit>
    {
        private readonly IBasketRepo _basketRepository;
        public DeleteBasketByUserNameHandler(IBasketRepo basketRepository)
        {
            _basketRepository = basketRepository;
        }
        public async Task<Unit> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
        {
            await _basketRepository.DeleteBasket(request.UserName);
            return Unit.Value;
        }
    }
}