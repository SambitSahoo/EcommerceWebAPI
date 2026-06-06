namespace Discount.Application.Handlers
{
    using Discount.Application.Queries;
    using Discount.Core.IRepo;
    using Discount.Grpc.Protos;
    using global::Grpc.Core;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetDiscountQueryHandler : IRequestHandler<GetDiscountQuery, CouponModel>
    {
        private readonly IDiscountRepo _repository;
        private readonly ILogger<GetDiscountQueryHandler> _logger;

        public GetDiscountQueryHandler(IDiscountRepo repository, ILogger<GetDiscountQueryHandler> logger )
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            }
            var couponModel = new CouponModel
            {
                Id = coupon.Id,
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount,
                ExpiryDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(coupon.ExpiryDate.ToUniversalTime()),
                IsActive = coupon.IsActive
            };
            _logger.LogInformation($"Coupon for the {request.ProductName} is fetched");
            return couponModel;
        }
    }
}