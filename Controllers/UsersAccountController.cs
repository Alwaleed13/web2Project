using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
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
            string role = HttpContext.Session.GetString("Role");

            if (role == "Customer")
            {
                var offers = _context.items.Where(x => x.Discount == "yes").ToList();
                return View(offers);
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Customer")
                {
                    var offers = _context.items.Where(x => x.Discount == "yes").ToList();
                    return View(offers);
                }
                else
                    return RedirectToAction("AdminHome", "UsersAccount");
            }
            else
            {
                return RedirectToAction("Login", "UsersAccount");
            }
        }

        // GET: UsersAccount/AdminHome
        public IActionResult AdminHome()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                return View();
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                    return View();
                else
                    return RedirectToAction("CustomerHome", "UsersAccount");
            }
            else
            {
                return RedirectToAction("Login", "UsersAccount");
            }
        }

        // GET: UsersAccount/Email
        public async Task<IActionResult> EmailAsync()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                ViewData["Message"] = "";
                return View();
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    ViewData["Message"] = "";
                    return View();
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // POST: UsersAccount/Email
        [HttpPost, ActionName("Email")]
        [ValidateAntiForgeryToken]
        public IActionResult Email(string address, string subject, string body)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("alwaleedhhabibib@gmail.com");
            mail.To.Add(address);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("alwaleedhhabib@gmail.com", "jsgwapsjfwhcjrem");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);

            ViewData["Message"] = "Email sent!";
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
                    return RedirectToAction("CustomerHome", "UsersAccount");
                else if (ro == "Admin")
                    return RedirectToAction("AdminHome", "UsersAccount");
                else
                    return View();
            }
        }

        // POST: UsersAccount/Login
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> Login(string na, string pa, string auto)
        {
            var ur = await _context.users_account.FromSqlRaw("SELECT * FROM users_account WHERE Name = {0} AND Password = {1}", na, pa).FirstOrDefaultAsync();
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
                    return RedirectToAction("CustomerHome", "UsersAccount");
                else if (ro == "Admin")
                    return RedirectToAction("AdminHome", "UsersAccount");
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
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("Name");
            HttpContext.Response.Cookies.Delete("Role");

            return RedirectToAction("Login", "UsersAccount");
        }



        // GET: UsersAccount/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: UsersAccount/Register
        [HttpPost, ActionName("Register")]
        public IActionResult Register([Bind("Name,Password,Email,Job,Married,Gender,Location")] Customer cu)
        {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=Project;Integrated Security=True;Trust Server Certificate=True");
            conn.Open();
            string sql;
            sql = "SELECT * FROM users_account WHERE Name = '" + cu.Name + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                ViewData["Message"] = "UserName already exists!";
                reader.Close();
            }
            else
            {
                reader.Close();
                sql = "INSERT INTO Customer (Name, Password, Email, Job, Married, Gender, Location) VALUES ('" + cu.Name + "','" +
                    cu.Password + "','" + cu.Email + "','" + cu.Job + "','" + cu.Married + "','" + cu.Gender + "','" + cu.Location + "')";

                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();

                string sqlAccount = "INSERT INTO users_account (Name, Password, Role, Pubdate) VALUES ('" + cu.Name + "','" + cu.Password + "', 'Customer', GETDATE())";

                SqlCommand commAccount = new SqlCommand(sqlAccount, conn);
                commAccount.ExecuteNonQuery();

                return RedirectToAction("Login");
            }
            conn.Close();
            return View();
        }

        //GET: UsersAccount/Search
        public IActionResult Search()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                List<UserAccount> userr = new List<UserAccount>();
                return View(userr);
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    List<UserAccount> userr = new List<UserAccount>();
                    return View(userr);
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // POST: UsersAccount/Search
        [HttpPost, ActionName("Search")]
        public async Task<IActionResult> Search(string user)
        {
            var userr = await _context.users_account.FromSqlRaw("select * from users_account where Name = '" + user + "' ").ToListAsync();

            return View(userr); ;
        }

        // GET: UsersAccount
        public async Task<IActionResult> Index()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                return View(await _context.users_account.Where(u => u.Role == "Customer").ToListAsync());
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    return View(await _context.users_account.Where(u => u.Role == "Customer").ToListAsync());
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // GET: UsersAccount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "UsersAccount");

            if (id == null) return NotFound();

            var user = await _context.users_account.FindAsync(id);
            if (user == null) return NotFound();

            var customer = await _context.customer.FirstOrDefaultAsync(c => c.Name == user.Name);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // GET: UsersAccount/Create(AdminAdd)
        public IActionResult Create()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                return View();
            }
            else if (HttpContext.Request.Cookies.ContainsKey("Role"))
            {
                role = HttpContext.Request.Cookies["Role"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();

                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);

                if (role == "Admin")
                {
                    return View();
                }
            }
            return RedirectToAction("Login", "UsersAccount");
        }

        // POST: UsersAccount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Password")] UserAccount usa)
        {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=Project;Integrated Security=True;Trust Server Certificate=True");
            conn.Open();
            string sql;
            sql = "SELECT * FROM users_account WHERE Name = '" + usa.Name + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                ViewData["Message"] = "This UserName is customer or already exists!";
                reader.Close();
            }
            else
            {
                reader.Close();
                sql = "INSERT INTO users_account (Name, Password, Role, Pubdate) VALUES ('" + usa.Name + "','" + usa.Password + "', 'Admin', GETDATE())";

                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();

                return RedirectToAction("AdminHome");
            }
            conn.Close();
            return View();
        }

        // GET: UsersAccount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "UsersAccount");

            if (id == null) return NotFound();

            var user = await _context.users_account.FindAsync(id);
            if (user == null) return NotFound();

            var customer = await _context.customer.FirstOrDefaultAsync(c => c.Name == user.Name);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: UsersAccount/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "UsersAccount");

            if (id != customer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    var loginAccount = await _context.users_account.FirstOrDefaultAsync(u => u.Name == customer.Name);
                    if (loginAccount != null)
                    {
                        loginAccount.Password = customer.Password;
                        _context.Update(loginAccount);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.customer.Any(e => e.Id == customer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: UsersAccount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "UsersAccount");

            if (id == null) return NotFound();

            var user = await _context.users_account.FindAsync(id);
            if (user == null) return NotFound();

            var customer = await _context.customer.FirstOrDefaultAsync(c => c.Name == user.Name);
            if (customer == null) return NotFound();

            ViewData["AccountId"] = id;
            return View(customer);
        }

        // POST: UsersAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("Login", "UsersAccount");

            var loginAccount = await _context.users_account.FindAsync(id);
            if (loginAccount != null)
            {
                var customer = await _context.customer.FirstOrDefaultAsync(c => c.Name == loginAccount.Name);
                if (customer != null)
                {
                    _context.customer.Remove(customer);
                }
                _context.users_account.Remove(loginAccount);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(int id)
        {
            return _context.users_account.Any(e => e.Id == id);
        }
    }
}
