using Microsoft.AspNetCore.Mvc;
using SmartBed.Data;
using SmartBed.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace SmartBed.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // LOGIN PAGE
        // ==========================================

        public IActionResult Index()
        {
            return View();
        }


        // ==========================================
        // REGISTER PAGE
        // ==========================================

        public IActionResult Register()
        {
            return View();
        }


        // ==========================================
        // LOGOUT
        // ==========================================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }


        // ==========================================
        // REGISTER USER
        // ==========================================

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["Success"] =
                    "Registration successful! Please login.";

                return RedirectToAction("Index");
            }

            return View(user);
        }


        // ==========================================
        // LOGIN
        // ==========================================

        [HttpPost]
        public IActionResult Index(
            string username,
            string password,
            string role)
        {

            // ==========================================
            // USERNAME VALIDATION
            // ==========================================

            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.Message = "Username is required.";
                return View();
            }

            username = username.Trim();

            if (username.Length < 3)
            {
                ViewBag.Message =
                    "Username must contain at least 3 characters.";

                return View();
            }

            if (username.Length > 50)
            {
                ViewBag.Message =
                    "Username cannot exceed 50 characters.";

                return View();
            }


            // ==========================================
            // PASSWORD VALIDATION
            // ==========================================

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "Password is required.";
                return View();
            }

            if (password.Length < 6)
            {
                ViewBag.Message =
                    "Password must contain at least 6 characters.";

                return View();
            }

            if (password.Length > 50)
            {
                ViewBag.Message =
                    "Password cannot exceed 50 characters.";

                return View();
            }


            // ==========================================
            // ROLE VALIDATION
            // ==========================================

            if (role != "Admin" &&
                role != "Hospital" &&
                role != "User")
            {
                ViewBag.Message =
                    "Please select a valid login role.";

                return View();
            }


            // ==========================================
            // ADMIN LOGIN
            // ==========================================

            if (role == "Admin")
            {
                var admin = _context.Admin.FirstOrDefault(a =>
                    a.Username == username &&
                    a.Password == password);

                if (admin != null)
                {
                    HttpContext.Session.SetInt32(
                        "AdminId",
                        admin.AdminId);

                    return RedirectToAction(
                        "Dashboard",
                        "Admin");
                }
            }


            // ==========================================
            // HOSPITAL LOGIN
            // ==========================================

            else if (role == "Hospital")
            {
                var hospital = _context.Hospital.FirstOrDefault(h =>
                    h.Username == username &&
                    h.Password == password);

                if (hospital != null)
                {
                    HttpContext.Session.SetInt32(
                        "HospitalId",
                        hospital.HospitalId);

                    return RedirectToAction(
                        "Dashboard",
                        "Hospital");
                }
            }


            // ==========================================
            // USER LOGIN
            // ==========================================

            else if (role == "User")
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username == username &&
                    u.Password == password);

                if (user != null)
                {
                    HttpContext.Session.SetInt32(
                        "UserId",
                        user.UserId);

                    return RedirectToAction(
                        "Dashboard",
                        "User");
                }
            }


            // ==========================================
            // INVALID LOGIN
            // ==========================================

            ViewBag.Message =
                "Invalid username, password, or login role.";

            return View();
        }
    }
}
