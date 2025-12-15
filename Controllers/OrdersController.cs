using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web2Project.Data;
using web2Project.Models;

namespace web2Project.Controllers
{
    public class OrdersController : Controller
    {
        private readonly web2ProjectContext _context;
        List<BuyItem> bitem = new List<BuyItem>();

        public OrdersController(web2ProjectContext context)
        {
            _context = context;
        }

        // GET: Orders/CatalogueBuy
        public async Task<IActionResult> CatalogueBuy()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Customer")
            {
                ViewData["Message"] = "";
                return View(await _context.items.OrderBy(x => x.Category).ToListAsync());
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Customer")
                {
                    ViewData["Message"] = "";
                    return View(await _context.items.OrderBy(x => x.Category).ToListAsync());
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        //GET: Orders/ItemBuyDetail
        public async Task<IActionResult> ItemBuyDetail(int? id)
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Customer")
            {
                var item = await _context.items.FindAsync(id);
                return View(item);
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Customer")
                {
                    var item = await _context.items.FindAsync(id);
                    return View(item);
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: Orders/CartBuy
        public async Task<IActionResult> CartBuy()
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) && HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);
            }
            if (role != "Customer")
            {
                return RedirectToAction("Login", "UsersAccount");
            }
            var sessionString = HttpContext.Session.GetString("Cart");
            List<BuyItem> bitem = new List<BuyItem>();
            if (sessionString is not null)
            {
                bitem = JsonSerializer.Deserialize<List<BuyItem>>(sessionString);
            }
            return View(bitem);
        }

        // POST: Orders/CartAdd
        [HttpPost]
        public async Task<IActionResult> CartAdd(int itemId, int quantity)
        {
            var item = await _context.items.FindAsync(itemId);

            if (item == null)
            {
                return NotFound();
            }
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            List<BuyItem> cart = new List<BuyItem>();
            if (sessionString != null)
            {
                cart = JsonSerializer.Deserialize<List<BuyItem>>(sessionString);
            }
            var existingItem = cart.FirstOrDefault(x => x.Id == itemId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new BuyItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = (decimal)item.Price,
                    Quantity = quantity,
                    ImgFile = item.ImgFile,
                    Description = item.Description
                });
            }
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            return RedirectToAction("CartBuy");
        }

        // GET: Orders/Buy
        public async Task<IActionResult> Buy()
        {
            await HttpContext.Session.LoadAsync();
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) && HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);
            }

            if (role != "Customer")
            {
                return RedirectToAction("Login", "UsersAccount");
            }
            var sessionString = HttpContext.Session.GetString("Cart");
            List<BuyItem> bitem = new List<BuyItem>();
            if (sessionString != null)
            {
                bitem = JsonSerializer.Deserialize<List<BuyItem>>(sessionString);
            }
            if (bitem.Count == 0)
            {
                TempData["Error"] = "Your cart is empty! Please add items first.";
                return RedirectToAction("CartBuy");
            }

            string tname = HttpContext.Session.GetString("Name");
            Order itorder = new Order
            {
                Name = tname,
                OrderDate = DateTime.Today,
                Total = 0
            };
            _context.orders.Add(itorder);
            await _context.SaveChangesAsync();
            decimal tot = 0;

            foreach (var it in bitem)
            {
                OrderLine oline = new OrderLine
                {
                    OrderId = itorder.Id,
                    ItemName = it.Name,
                    Quantity = it.Quantity,
                    Price = it.Price
                };
                _context.order_line.Add(oline);

                var product = await _context.items.FindAsync(it.Id);
                if (product != null)
                {
                    product.Quantity = product.Quantity - it.Quantity;
                    _context.Update(product);
                }
                tot += (it.Quantity * it.Price);
            }

            await _context.SaveChangesAsync();
            itorder.Total = (int)tot;
            _context.Update(itorder);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");
            TempData["Message"] = "Thank you!";

            return RedirectToAction("MyOrder");
        }

        // GET: Orders/MyOrder
        public async Task<IActionResult> MyOrder()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Customer")
            {
                string tname = HttpContext.Session.GetString("Name");
                return View(await _context.orders.Where(x => x.Name == tname).ToListAsync());
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Customer")
                {
                    string tname = HttpContext.Session.GetString("Name");
                    return View(await _context.orders.Where(x => x.Name == tname).ToListAsync());
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: Orders/OrderLine
        public async Task<IActionResult> OrderLine(int? orid)
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Customer")
            {
                var buybk = await _context.order_line.Where(x => x.OrderId == orid).ToListAsync();
                return View(buybk);
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Customer")
                {
                    var buybk = await _context.order_line.Where(x => x.OrderId == orid).ToListAsync();
                    return View(buybk);
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: Orders/OrderDetail
        public async Task<IActionResult> OrderDetail(string Name)
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                if (Name == null) return NotFound();

                var customerOrders = await _context.orders.Where(o => o.Name == Name).ToListAsync();
                return View(customerOrders);
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    if (Name == null) return NotFound();

                    var customerOrders = await _context.orders.Where(o => o.Name == Name).ToListAsync();
                    return View(customerOrders);
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                var orItems = await _context.report.FromSqlRaw("SELECT Name, SUM(Total) as Total FROM orders GROUP BY Name  ").ToListAsync();
                return View(orItems);
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    var orItems = await _context.report.FromSqlRaw("SELECT Name, SUM(Total) as Total FROM orders GROUP BY Name  ").ToListAsync();
                    return View(orItems);
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,OrderDate,Total")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,OrderDate,Total")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString != null)
            {
                List<BuyItem> cart = JsonSerializer.Deserialize<List<BuyItem>>(sessionString);
                var itemToRemove = cart.FirstOrDefault(x => x.Id == id);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                    HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
                }
            }
            return RedirectToAction("CartBuy");
        }

        private bool OrderExists(int id)
        {
            return _context.orders.Any(e => e.Id == id);
        }
    }
}
