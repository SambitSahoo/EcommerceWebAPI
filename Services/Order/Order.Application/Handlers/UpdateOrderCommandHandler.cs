using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Commands;
using Order.Infrastructure.Context;
using Order.Infrastructure.Entities;

namespace Order.Application.Handlers
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly OrderDbContext _context;

        public UpdateOrderCommandHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.PurchaseOrders
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.Id} not found.");

            order.UserName = request.UserName;
            order.TotalPrice = request.TotalPrice;
            order.FirstName = request.FirstName;
            order.LastName = request.LastName;
            order.EmailAddress = request.EmailAddress;
            order.AddressLine = request.AddressLine;
            order.Country = request.Country;
            order.State = request.State;
            order.ZipCode = request.ZipCode;
            order.CardName = request.CardName;
            order.CardNumber = request.CardNumber;
            order.Expiration = request.Expiration;
            order.CVV = request.CVV;
            order.PaymentMethod = request.PaymentMethod;
            order.CreatedBy = request.CreatedBy;
            order.LastModifiedBy = request.LastModifiedBy;
            order.LastModifiedDate = DateTime.UtcNow;

            _context.PurchaseOrders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
