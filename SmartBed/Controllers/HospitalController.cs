using Microsoft.AspNetCore.Mvc;
using SmartBed.Data;
using SmartBed.Models;
using Microsoft.AspNetCore.SignalR;
using SmartBed.Hubs;
using Microsoft.AspNetCore.Http;

namespace SmartBed.Controllers
{
    public class HospitalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<BedHub> _hubContext;

        public HospitalController(
            ApplicationDbContext context,
            IHubContext<BedHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }


        // Hospital Dashboard
        public IActionResult Dashboard()
        {
            int? hospitalId =
                HttpContext.Session.GetInt32("HospitalId");

            if (hospitalId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var hospital = _context.Hospital
                .FirstOrDefault(h => h.HospitalId == hospitalId);

            if (hospital == null)
            {
                return NotFound();
            }

            // Get bookings for this hospital only
            var bookings = _context.Bookings
                .Where(b => b.HospitalId == hospitalId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            ViewBag.Bookings = bookings;

            return View(hospital);
        }


        // Update Bed Availability
        [HttpPost]
        public async Task<IActionResult> UpdateBeds(Hospital hospital)
        {
            // Check whether hospital is logged in
            int? hospitalId = HttpContext.Session.GetInt32("HospitalId");

            if (hospitalId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Prevent negative bed values
            if (hospital.ICUBeds < 0 ||
                hospital.EmergencyBeds < 0 ||
                hospital.GeneralBeds < 0)
            {
                ModelState.AddModelError(
                    "",
                    "Bed availability cannot be a negative number."
                );
            }

            // Check model validation
            if (!ModelState.IsValid)
            {
                var existingHospital = _context.Hospital
                    .FirstOrDefault(h => h.HospitalId == hospitalId.Value);

                if (existingHospital == null)
                {
                    return NotFound();
                }

                return View("Dashboard", existingHospital);
            }

            // Find hospital from database
            var data = _context.Hospital
                .FirstOrDefault(h => h.HospitalId == hospitalId.Value);

            if (data == null)
            {
                return NotFound();
            }

            // Update bed availability
            data.ICUBeds = hospital.ICUBeds;
            data.EmergencyBeds = hospital.EmergencyBeds;
            data.GeneralBeds = hospital.GeneralBeds;

            _context.SaveChanges();

            // Send real-time update
            await _hubContext.Clients.All.SendAsync("ReceiveBedUpdate");

            TempData["Success"] = "Bed availability updated successfully.";

            return RedirectToAction("Dashboard");
        }
    }
}
