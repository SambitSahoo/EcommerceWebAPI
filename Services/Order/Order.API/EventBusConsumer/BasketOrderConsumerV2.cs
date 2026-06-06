using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Order.Application.Commands;

namespace Order.API.EventBusConsumer
{
    public class BasketOrderConsumerV2 : IConsumer<BasketCheckoutEventV2>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketOrderConsumerV2> _logger;

        public BasketOrderConsumerV2(IMediator mediator, IMapper mapper, ILogger<BasketOrderConsumerV2> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEventV2> context)
        {
            using var scope = _logger.BeginScope("Consuming Basket Checkout Event for {CorelationId}", context.Message.CorelationId);
            var cmd = _mapper.Map<CheckoutOrderCommandV2>(context.Message);
            var result = await _mediator.Send(cmd);
            _logger.LogInformation("Basket Checkout Event Completed!");
        }
    }
}
 