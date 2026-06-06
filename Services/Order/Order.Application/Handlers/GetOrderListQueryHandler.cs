using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Queries;
using Order.Application.Responses;
using Order.Infrastructure.Context;
using Order.Infrastructure.Entities;

namespace Order.Application.Handlers
{
    public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, List<OrderResponse>>
    {
        private readonly OrderDbContext _context;
        private readonly IMapper _mapper;

        public GetOrderListQueryHandler(OrderDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OrderResponse>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.PurchaseOrders
                .Where(o => o.UserName == request.UserName)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<OrderResponse>>(orders);
        }
    }
}
