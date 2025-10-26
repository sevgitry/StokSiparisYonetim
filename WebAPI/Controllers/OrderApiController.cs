using BusinessLayer.Services;
using EntityLayer.DTOs;
using EntityLayer.DTOs.Order;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderApiController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderApiController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderResponseDto>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderCreateDto orderDto)
        {
            var order = await _orderService.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] OrderUpdateDto orderDto)
        {
            if (id != orderDto.Id) return BadRequest();

            var result = await _orderService.UpdateOrderStatusAsync(id, orderDto.Status, orderDto.RowVersion);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}