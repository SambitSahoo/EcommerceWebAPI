using MediatR;

namespace Order.Application.Commands
{
    public class CheckoutOrderCommandV2 : IRequest<int>
    {
        public string? UserName { get; set; }
        public decimal TotalPrice { get; set; }
    }
    
}