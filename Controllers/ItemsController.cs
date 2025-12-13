using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using web2Project.Data;
using web2Project.Models;

namespace web2Project.Controllers
{
    public class ItemsController : Controller
    {
        private readonly web2ProjectContext _context;

        public ItemsController(web2ProjectContext context)
        {
            _context = context;
        }

        // GET: Items/List
        public async Task<IActionResult> List()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                return View(await _context.items.OrderBy(x => x.Category).ToListAsync());
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    return View(await _context.items.OrderBy(x => x.Category).ToListAsync());
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: UsersAccount/statis
        public IActionResult Statis()
        {
            string role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role) && HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);
            }
            if (role == "Admin")
            {
                SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=Project;Integrated Security=True;Trust Server Certificate=True");
                conn.Open();
                string sql;
                SqlCommand comm;

                sql = "SELECT COUNT(Id) FROM items where Category ='Medicine'";
                comm = new SqlCommand(sql, conn);
                ViewData["d1"] = Convert.ToInt32(comm.ExecuteScalar());

                sql = "SELECT COUNT(Id) FROM items where Category ='Personal Care'";
                comm = new SqlCommand(sql, conn);
                ViewData["d2"] = Convert.ToInt32(comm.ExecuteScalar());

                sql = "SELECT COUNT(Id) FROM items where Category ='Vitamins & Supplements'";
                comm = new SqlCommand(sql, conn);
                ViewData["d3"] = Convert.ToInt32(comm.ExecuteScalar());

                conn.Close();
                return View();
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                return View(await _context.items.ToListAsync());
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    return View(await _context.items.ToListAsync());
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }


        // GET: Items/Details/5(Customer only see this)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,Name,Description,Price,Discount,Category,Quantity")] Item item)
        {
            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                item.ImgFile = filename;
            }

            _context.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile file, int id, [Bind("Id,Name,Description,Price,Discount,Category,Quantity,ImgFile")] Item item)
        {
            if (id != item.Id)
            { return NotFound(); }

            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                item.ImgFile = filename;
            }

            _context.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.items.FindAsync(id);
            if (item != null)
            {
                _context.items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.items.Any(e => e.Id == id);
        }
    }
}
