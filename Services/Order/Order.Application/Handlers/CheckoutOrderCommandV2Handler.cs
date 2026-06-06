using MediatR;
using Order.Application.Commands;
using Order.Infrastructure.Context;
using Order.Infrastructure.Entities;

namespace Order.Application.Handlers
{
    public class CheckoutOrderCommandV2Handler : IRequestHandler<CheckoutOrderCommandV2, int>
    {
        private readonly OrderDbContext _context;

        public CheckoutOrderCommandV2Handler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CheckoutOrderCommandV2 request, CancellationToken cancellationToken)
        {
            var order = new PurchaseOrder
            {
                UserName = request.UserName,             
                TotalPrice = request.TotalPrice
            };

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
