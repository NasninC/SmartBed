using Microsoft.AspNetCore.Mvc;
using SmartBed.Data;
using SmartBed.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace SmartBed.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Login Page
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Index(string username, string password, string role)
        {
            if (role == "Admin")
            {
                var admin = _context.Admin.FirstOrDefault(a =>
                    a.Username == username && a.Password == password);

                if (admin != null)
                {
                    HttpContext.Session.SetInt32("AdminId", admin.AdminId);
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            else if (role == "Hospital")
            {
                var hospital = _context.Hospital.FirstOrDefault(h =>
                    h.Username == username && h.Password == password);

                if (hospital != null)
                {
                    HttpContext.Session.SetInt32("HospitalId", hospital.HospitalId);
                    return RedirectToAction("Dashboard", "Hospital");
                }
            }
            else if (role == "User")
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username == username && u.Password == password);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction("Dashboard", "User");
                }
            }

            ViewBag.Message = "Invalid Username or Password";
            return View();
        }
    }
}