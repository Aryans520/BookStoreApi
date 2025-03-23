using BookStoreApi.Dtos;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // نیاز به احراز هویت
    public class OrdersController : Controller
    {
        private readonly BookStoreApiDbContext _context;

        public OrdersController(BookStoreApiDbContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return await _context.Orders.Where(o => o.UserId == userId).Include(o => o.OrderItems).ThenInclude(oi => oi.Book).ToListAsync();
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderCreateDto orderDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = orderDto.OrderItems.Sum(oi => oi.Price * oi.Quantity),
                OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
                {
                    BookId = oi.BookId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Book).FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            return order;
        }
    }
}
