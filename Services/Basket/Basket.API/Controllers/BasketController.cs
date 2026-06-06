using System.Net;
using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;


namespace Basket.API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IMediator mediator, IPublishEndpoint publishEndpoint,ILogger<BasketController> logger)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _logger= logger;
        }
        /// <summary>
        /// Retrieves the shopping basket for a given user.
        /// </summary>
        /// <param name="userName">The username associated with the basket.</param>
        /// <returns>The shopping cart details if found; otherwise, an error response.</returns>
        /// <response code="200">Returns the basket for the specified user.</response>
        /// <response code="404">Basket not found for the specified user.</response>
        /// <response code="500">An unexpected error occurred while retrieving the basket.</response>
        [HttpGet("GetBasket/{userName}")]
        [ProducesResponseType(typeof(ShoppingCartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShoppingCartResponse>> GetBasket(string userName)
        {
            userName = userName?.Trim().ToLower();
            try
            {
                var query = new GetBasketByUserNameQuery(userName);
                var basket = await _mediator.Send(query);
                if (basket == null)
                {
                    return NotFound($"Basket not found for user: {userName}");
                }
                return Ok(basket);
            }
            catch (Exception)
            {
                // Log the exception (not shown here for brevity)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        /// <summary>
        /// Creates or updates the shopping basket for a user.
        /// </summary>
        /// <param name="createShoppingCartCommand">The basket data to be created or updated.</param>
        /// <returns>The updated shopping cart details.</returns>
        /// <response code="200">Basket created or updated successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="500">An unexpected error occurred while updating the basket.</response>
        [HttpPost("CreateBasket")]
        [ProducesResponseType(typeof(ShoppingCartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShoppingCartResponse>> UpdateBasket([FromBody] CreateShoppingCartCommand createShoppingCartCommand)
        {
            try
            {
                if (createShoppingCartCommand == null)
                {
                    return BadRequest("Basket data is required.");
                }
                createShoppingCartCommand.UserName = createShoppingCartCommand.UserName?.Trim().ToLower();
                var basket = await _mediator.Send(createShoppingCartCommand);
                return Ok(basket);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating basket: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the shopping basket for a specified user.
        /// </summary>
        /// <param name="userName">The username whose basket should be deleted.</param>
        /// <returns>Confirmation of deletion.</returns>
        /// <response code="200">Basket deleted successfully.</response>
        /// <response code="404">Basket not found for the specified user.</response>
        /// <response code="500">An unexpected error occurred while deleting the basket.</response>
        [HttpDelete("DeleteBasket/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            userName = userName?.Trim().ToLower();
            try
            {
                var command = new DeleteBasketByUserNameCommand(userName);
                await _mediator.Send(command);

                return Ok($"Basket for user '{userName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting basket: {ex.Message}");
            }
        }
        /// <summary>
        /// Processes the checkout for a user's basket by publishing an event to the message bus
        /// and removing the basket from the cache.
        /// </summary>
        /// <param name="basketCheckout">The checkout details including user information and payment data.</param>
        /// <returns>Returns an HTTP 202 Accepted response if successful; otherwise, a BadRequest.</returns>
        /// <response code="202">Checkout event published and basket removed successfully.</response>
        /// <response code="400">Basket not found or invalid checkout data.</response>
        /// <response code="500">An unexpected error occurred during checkout.</response>
        [HttpPost("Checkout")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            if (basketCheckout == null || string.IsNullOrWhiteSpace(basketCheckout.UserName))
            {
                return BadRequest("Invalid checkout data.");
            }

            basketCheckout.UserName = basketCheckout.UserName.Trim().ToLower();

            try
            {
                //get the existing basket with Username
                var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
                var basket = await _mediator.Send(query);

                if (basket == null)
                {
                    return BadRequest($"Basket not found for user: {basketCheckout.UserName}");
                }

                var eventMessage = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
                eventMessage.TotalPrice = basket.TotalPrice;

                await _publishEndpoint.Publish(eventMessage);
                _logger.LogInformation($"Basket published for {basket.UserName}");
                
                //Remove the basket
                var deleteCmd = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
                await _mediator.Send(deleteCmd);

                return Accepted();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Checkout failed: {ex.Message}");
            }
        }

    }
}



