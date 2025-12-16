using Microsoft.AspNetCore.Mvc;
using web2Project.Models;
using web2Project.Data;
using System.Linq;

namespace web2Project.Controllers
{
    [Route("api")]
    [ApiController]
    public class MobileApiController : ControllerBase
    {
        private readonly web2ProjectContext _context;

        public MobileApiController(web2ProjectContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestData data)
        {
            var user = _context.users_account
                .FirstOrDefault(u => u.Name == data.Username && u.Password == data.Password);

            if (user != null) return Ok(new { message = "Login Successful" });
            return Unauthorized();
        }

        [HttpGet("items")]
        public IActionResult GetItems(string? category = null)
        {
            var query = _context.items.AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                query = query.Where(i => i.Category == category);
            }

            var items = query.Select(i => new
            {
                Id = i.Id,
                ItemName = i.Name,
                Price = i.Price,
                Description = i.Description,
                ImgFile = i.ImgFile,
                Quantity = i.Quantity,
                Category = i.Category
            }).ToList();

            return Ok(items);
        }

        [HttpPost("buy")]
        public IActionResult Buy([FromBody] BuyRequest request)
        {
            var item = _context.items.FirstOrDefault(i => i.Id == request.ItemId);
            if (item == null) return NotFound("Item not found");
            if (item.Quantity <= 0) return BadRequest("Out of Stock");

            try
            {
                var order = new Order
                {
                    Name = request.UserName,
                    Total = (int)item.Price,
                    OrderDate = DateTime.Now
                };
                _context.orders.Add(order);
                _context.SaveChanges();

                var line = new OrderLine
                {
                    OrderId = order.Id,
                    ItemName = item.Name,
                    Quantity = 1,
                    Price = (decimal)item.Price
                };
                _context.order_line.Add(line);

                item.Quantity = item.Quantity - 1;
                _context.items.Update(item);
                _context.SaveChanges();

                return Ok(new { message = "Purchase Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class LoginRequestData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class BuyRequest
    {
        public int ItemId { get; set; }
        public string UserName { get; set; }
    }
}