using AutoMapper;
using EventBus.Messages.Events;
using Order.Application.Commands;
using Order.Application.Responses;
using Order.Infrastructure.Entities;

namespace Order.Application.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<PurchaseOrder, OrderResponse>().ReverseMap();
            CreateMap<PurchaseOrder, CheckoutOrderCommand>().ReverseMap();
            CreateMap<PurchaseOrder, UpdateOrderCommand>().ReverseMap();
            CreateMap<PurchaseOrder, CheckoutOrderCommandV2>().ReverseMap();
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
            CreateMap<CheckoutOrderCommandV2, BasketCheckoutEventV2>().ReverseMap();
        }
    }
}
