using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Commands;
using Order.Infrastructure.Context;
using Order.Infrastructure.Entities;

namespace Order.Application.Handlers
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly OrderDbContext _context;

        public DeleteOrderCommandHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.PurchaseOrders
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.Id} not found.");

            _context.PurchaseOrders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
