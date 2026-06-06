using MediatR;
using Order.Application.Commands;
using Order.Infrastructure.Context;
using Order.Infrastructure.Entities;

namespace Order.Application.Handlers
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly OrderDbContext _context;

        public CheckoutOrderCommandHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new PurchaseOrder
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailAddress = request.EmailAddress,
                AddressLine = request.AddressLine,
                Country = request.Country,
                State = request.State,
                ZipCode = request.ZipCode,
                CardName = request.CardName,
                CardNumber = request.CardNumber,
                CVV = request.CVV,
                Expiration = request.Expiration,
                PaymentMethod = request.PaymentMethod,
                CreatedBy = request.CreatedBy,
                LastModifiedBy = request.LastModifiedBy,
                LastModifiedDate = DateTime.UtcNow,
                TotalPrice = request.TotalPrice
            };

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
