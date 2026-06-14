using Asp.Versioning;
using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Specs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    /// <summary>
    /// Controller for managing catalog products, brands, and types.
    /// Provides endpoints for CRUD operations and queries.
    /// </summary>
    [ApiController]
    [Asp.Versioning.ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogController"/>.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling queries and commands.</param>
        public CatalogController(IMediator mediator,ILogger<CatalogController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique product identifier.</param>
        /// <returns>A product matching the given identifier.</returns>
        [HttpGet]
        [Route("[action]/{id}", Name = "GetProductById")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> GetProductById(string id)
        {
            var response = new GetProductByIdQuery(id);
            var result = await _mediator.Send(response);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves products by their name.
        /// </summary>
        /// <param name="productName">The name of the product(s).</param>
        /// <returns>A list of products matching the name.</returns>
        [HttpGet]
        [Route("[action]/{productName}", Name = "GetProductByName")]
        [ProducesResponseType(typeof(IList<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> GetProductByName(string productName)
        {
            var response = new GetProductByNameQuery(productName);
            var result = await _mediator.Send(response);
            _logger.LogInformation($"Product with {productName} fetched");
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all products from the catalog.
        /// </summary>
        /// <returns>A list of all products.</returns>
        [HttpGet]
        [Route("GetAllProducts", Name = "GetAllProducts")]
        [ProducesResponseType(typeof(Pagination<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<ProductResponse>>> GetAllProducts([FromQuery] CatalogSpecParams specParams)
        {
            var response = new GetAllProductsQuery(specParams);
            var result = await _mediator.Send(response);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all available product brands.
        /// </summary>
        /// <returns>A list of product brands.</returns>
        [HttpGet]
        [Route("GetAllBrands", Name = "GetAllBrands")]
        [ProducesResponseType(typeof(IList<BrandResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<BrandResponse>>> GetAllBrands()
        {
            var response = new GetAllBrandsQuery();
            var result = await _mediator.Send(response);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all available product types.
        /// </summary>
        /// <returns>A list of product types.</returns>
        [HttpGet]
        [Route("GetAllTypes", Name = "GetAllTypes")]
        [ProducesResponseType(typeof(IList<TypesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<TypesResponse>>> GetAllTypes()
        {
            var response = new GetAllTypesQuery();
            var result = await _mediator.Send(response);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves products filtered by brand name.
        /// </summary>
        /// <param name="brand">The brand name.</param>
        /// <returns>A list of products belonging to the specified brand.</returns>
        [HttpGet]
        [Route("[action]/{brand}", Name = "GetProductsByBrandName")]
        [ProducesResponseType(typeof(IList<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<ProductResponse>>> GetProductsByBrandName(string brand)
        {
            var response = new GetProductByBrandQuery(brand);
            var result = await _mediator.Send(response);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new product in the catalog.
        /// </summary>
        /// <param name="productCommand">The product details.</param>
        /// <returns>The created product.</returns>
        [HttpPost]
        [Route("CreateProduct", Name = "CreateProduct")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductCommand productCommand)
        {
            var result = await _mediator.Send(productCommand);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing product in the catalog.
        /// </summary>
        /// <param name="productCommand">The updated product details.</param>
        /// <returns>A boolean indicating success.</returns>
        [HttpPut]
        [Route("UpdateProduct", Name = "UpdateProduct")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> UpdateProduct([FromBody] UpdateProductCommand productCommand)
        {
            var result = await _mediator.Send(productCommand);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a product from the catalog by its identifier.
        /// </summary>
        /// <param name="id">The unique product identifier.</param>
        /// <returns>A boolean indicating whether the product was successfully deleted.</returns>
        [HttpDelete]
        [Route("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeleteResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeleteResponse>> DeleteProduct(string id)
        {
            var response = new DeleteProductByIdCommand(id);
            var result = await _mediator.Send(response);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
