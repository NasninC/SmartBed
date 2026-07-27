using Microsoft.AspNetCore.Mvc;
using SmartBed.Data;
using SmartBed.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace SmartBed.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all hospitals
        public IActionResult Dashboard()
        {
            ViewBag.TotalHospitals = _context.Hospital.Count();
            ViewBag.TotalUsers = _context.Users.Count();

            ViewBag.TotalICUBeds = _context.Hospital.Sum(h => h.ICUBeds);
            ViewBag.TotalEmergencyBeds = _context.Hospital.Sum(h => h.EmergencyBeds);
            ViewBag.TotalGeneralBeds = _context.Hospital.Sum(h => h.GeneralBeds);

            var hospitals = _context.Hospital.ToList();

            return View(hospitals);
        }

        // Show Add Hospital page
        public IActionResult Create()
        {
            return View();
        }

        // Save Hospital
        [HttpPost]
        public IActionResult Create(Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                _context.Hospital.Add(hospital);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(hospital);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var hospital = _context.Hospital.Find(id);

            if (hospital == null)
            {
                return NotFound();
            }

            return View(hospital);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Edit(Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                _context.Hospital.Update(hospital);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            return View(hospital);
        }
        public IActionResult Delete(int id)
        {
            var hospital = _context.Hospital.Find(id);

            if (hospital != null)
            {
                _context.Hospital.Remove(hospital);
                _context.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }

    }
}