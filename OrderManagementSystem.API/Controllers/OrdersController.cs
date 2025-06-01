using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Application.DTOs;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Models;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.API.Controllers
{
    /// <summary>
    /// API controller for managing orders.
    /// </summary>
    /// 
    //[Authorize(Roles = "Admin")] - if the API requires authentication 
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<OrdersController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        public OrdersController(IOrderService orderService, ICustomerService customerService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="dto">The order details.</param>
        /// <returns>The created order with generated ID and details.</returns>
        /// <response code="201">Returns the newly created order.</response>
        /// <response code="400">If the request is invalid or the customer doesn't exist.</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponseDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] OrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerExists = await _customerService.CustomerExistsAsync(dto.CustomerId);
            if (!customerExists)
                return BadRequest($"Customer with ID {dto.CustomerId} does not exist.");

            var orderEntity = new Order
            {
                CustomerId = dto.CustomerId,
                Items = dto.Items
                    .Select(i => new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    })
                    .ToList()
            };

            try
            {
                var created = await _orderService.CreateOrderAsync(orderEntity);

                var responseDto = new OrderResponseDto
                {
                    Id = created.Id,
                    CustomerId = created.CustomerId,
                    Items = created.Items?.Select(i => new OrderItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                };

                var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
                return CreatedAtAction(nameof(GetById), new { version, id = responseDto.Id }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an order.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>The order details if found.</returns>
        /// <response code="200">Returns the order.</response>
        /// <response code="404">If the order is not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var responseDto = new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return Ok(responseDto);
        }

        /// <summary>
        /// Gets the discount applied to a specific order.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>The discount result.</returns>
        /// <response code="200">Returns the discount result.</response>
        /// <response code="404">If the order is not found.</response>
        [HttpGet("{id:guid}/discount")]
        [ProducesResponseType(typeof(DiscountResult), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDiscount(Guid id)
        {
            try
            {
                var discount = await _orderService.GetDiscountForOrderAsync(id);
                return Ok(discount);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <param name="newStatus">The new status to apply.</param>
        /// <returns>No content if successful; otherwise a suitable error.</returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="404">If the order was not found or transition was invalid.</response>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] OrderStatus newStatus)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, newStatus);
            return success
                ? NoContent()
                : NotFound(new { message = "Order not found or invalid transition" });
        }

        /// <summary>
        /// Retrieves analytics summary for all orders.
        /// </summary>
        /// <returns>Order analytics including totals and averages.</returns>
        /// <response code="200">Returns the analytics summary.</response>
        [HttpGet("analytics")]
        [ProducesResponseType(typeof(OrderAnalyticsDto), 200)]
        public async Task<IActionResult> GetAnalytics()
        {
            var analytics = await _orderService.GetOrderAnalyticsAsync();
            return Ok(analytics);
        }
    }
}
