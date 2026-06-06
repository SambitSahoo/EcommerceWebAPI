using AutoMapper;
using Discount.Application.Commands;
using Discount.Core.Entities;
using Discount.Core.IRepo;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Handlers
{
    public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, CouponModel>
    {
        private readonly IDiscountRepo _repository;
        private readonly IMapper _mapper;
        public CreateDiscountCommandHandler(IDiscountRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        } 
        public async Task<CouponModel> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var coupon = _mapper.Map<Coupon>(request);
            await _repository.CreateDiscount(coupon);
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }
    }
}