using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web2Project.Data;
using web2Project.Models;

namespace web2Project.Controllers
{
    public class UsersAccountController : Controller
    {
        private readonly web2ProjectContext _context;

        public UsersAccountController(web2ProjectContext context)
        {
            _context = context;
        }

        // GET: UsersAccount/CustomerHome
        public IActionResult CustomerHome()
        {
            return View();
        }

        // GET: UsersAccount/AdminHome
        public IActionResult AdminHome()
        {
            return View();
        }

        // GET: UsersAccount/Email
        public IActionResult Email ()
        {
            return View();
        }

        // POST: UsersAccount/Email
        [HttpPost, ActionName("Email")]
        public async Task<IActionResult> Email(string auto)
        {
            return View();
        }

        // GET: UsersAccount/Login
        public IActionResult Login()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Name"))
                return View();
            else
            {
                string na = HttpContext.Request.Cookies["Name"].ToString();
                string ro = HttpContext.Request.Cookies["Role"].ToString();
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", ro);

                if (ro == "Customer")
                    return RedirectToAction("UsersAccount", "CustomerHome");
                else if (ro == "Admin")
                    return RedirectToAction("UsersAccount", "AdminHome");
                else
                    return View();
            }
        }

        // POST: UsersAccount/Login
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> Login(string na, string pa, string auto)
        {
            var ur = await _context.UserAccount.FromSqlRaw("SELECT * FROM users_account WHERE Name = {0} AND Password = {1}", na, pa).FirstOrDefaultAsync();
            if (ur != null)
            {
                int id = ur.Id;
                string nal = ur.Name;
                string ro = ur.Role;
                HttpContext.Session.SetString("userId", Convert.ToString(id));
                HttpContext.Session.SetString("Name", nal);
                HttpContext.Session.SetString("Role", ro);

                if (auto == "on")
                {
                    HttpContext.Response.Cookies.Append("Name", nal);
                    HttpContext.Response.Cookies.Append("Role", ro);
                }
                if (ro == "Customer")
                    return RedirectToAction("UsersAccount", "CustomerHome");
                else if (ro == "Admin")
                    return RedirectToAction("UsersAccount", "AdminHome");
                else
                    return View();
            }
            else
            {
                ViewData["Message"] = "Wrong UserName or Password!";
                return View();
            }
        }

        // POST: UsersAccount/Logout
        [HttpPost, ActionName("Logout")]
        public async Task<IActionResult> Logout()
        {
            return View();
        }

        // GET: UsersAccount/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: UsersAccount/Register
        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> Register(string auto)
        {
            return View();
        }

        //GET: UsersAccount/SearchAll
        public IActionResult SearchAll()
        {
            return View();
        }

        // POST: UsersAccount/SearchAll
        [HttpPost, ActionName("SearchAll")]
        public async Task<IActionResult> SearchAll(string auto)
        {
            return View();
        }

        // GET: UsersAccount
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserAccount.ToListAsync());
        }

        // GET: UsersAccount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // GET: UsersAccount/Create(AdminAdd)
        public IActionResult Create()
        {
            return View();
        }

        // POST: UsersAccount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userAccount);
        }

        // GET: UsersAccount/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccount.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }
            return View(userAccount);
        }

        // POST: UsersAccount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] UserAccount userAccount)
        {
            if (id != userAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAccountExists(userAccount.Id))
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
            return View(userAccount);
        }

        // GET: UsersAccount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // POST: UsersAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userAccount = await _context.UserAccount.FindAsync(id);
            if (userAccount != null)
            {
                _context.UserAccount.Remove(userAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(int id)
        {
            return _context.UserAccount.Any(e => e.Id == id);
        }
    }
}
