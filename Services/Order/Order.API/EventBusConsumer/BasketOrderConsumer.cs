using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Order.Application.Commands;

namespace Order.API.EventBusConsumer
{
    public class BasketOrderConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketOrderConsumer> _logger;
        public BasketOrderConsumer(IMediator mediator, IMapper mapper, ILogger<BasketOrderConsumer> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            using var scope = _logger.BeginScope("Consuming Basket Checkout Event for {CorelationId}", context.Message.CorelationId);
            var cmd = _mapper.Map<CheckoutOrderCommand>(context.Message);
            var result = await _mediator.Send(cmd);
            _logger.LogInformation("Basket Checkout Event Completed!");
        }
    }
}