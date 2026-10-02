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


        // ==========================================
        // HOSPITAL DASHBOARD
        // ==========================================

        public IActionResult Dashboard()
        {
            int? hospitalId =
                HttpContext.Session.GetInt32("HospitalId");

            if (hospitalId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var hospital = _context.Hospital
                .FirstOrDefault(h => h.HospitalId == hospitalId.Value);

            if (hospital == null)
            {
                return NotFound();
            }

            // Get bookings for this hospital only
            var bookings = _context.Bookings
                .Where(b => b.HospitalId == hospitalId.Value)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            ViewBag.Bookings = bookings;

            return View(hospital);
        }


        // ==========================================
        // UPDATE BED AVAILABILITY
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> UpdateBeds(
            int hospitalId,
            int ICUBeds,
            int EmergencyBeds,
            int GeneralBeds)
        {
            // Check whether hospital is logged in
            int? loggedHospitalId =
                HttpContext.Session.GetInt32("HospitalId");

            if (loggedHospitalId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Security check:
            // Hospital can update only its own beds
            if (hospitalId != loggedHospitalId.Value)
            {
                return Unauthorized();
            }


            // ==========================================
            // VALIDATE BED VALUES
            // ==========================================

            if (ICUBeds < 0 ||
                EmergencyBeds < 0 ||
                GeneralBeds < 0)
            {
                TempData["Error"] =
                    "Bed availability cannot be a negative number.";

                return RedirectToAction("Dashboard");
            }


            if (ICUBeds > 10000 ||
                EmergencyBeds > 10000 ||
                GeneralBeds > 10000)
            {
                TempData["Error"] =
                    "Bed availability cannot exceed 10,000.";

                return RedirectToAction("Dashboard");
            }


            // ==========================================
            // FIND HOSPITAL
            // ==========================================

            var hospital = _context.Hospital
                .FirstOrDefault(h => h.HospitalId == loggedHospitalId.Value);

            if (hospital == null)
            {
                return NotFound();
            }


            // ==========================================
            // UPDATE BED COUNTS
            // ==========================================

            hospital.ICUBeds = ICUBeds;
            hospital.EmergencyBeds = EmergencyBeds;
            hospital.GeneralBeds = GeneralBeds;


            // ==========================================
            // SAVE TO DATABASE
            // ==========================================

            _context.SaveChanges();


            // ==========================================
            // SIGNALR REAL-TIME UPDATE
            // ==========================================

            await _hubContext.Clients.All
                .SendAsync("ReceiveBedUpdate");


            TempData["Success"] =
                "Bed availability updated successfully.";


            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        public IActionResult CompleteBooking(int bookingId)
        {
            int? hospitalId = HttpContext.Session.GetInt32("HospitalId");

            if (hospitalId == null)
                return RedirectToAction("Index", "Login");

            var booking = _context.Bookings
                .FirstOrDefault(b =>
                    b.BookingId == bookingId &&
                    b.HospitalId == hospitalId.Value);

            if (booking == null)
                return NotFound();

            booking.Status = "Completed";

            _context.SaveChanges();

            TempData["Success"] = "Booking marked as completed successfully.";

            return RedirectToAction("Dashboard");
        }
    }
}
