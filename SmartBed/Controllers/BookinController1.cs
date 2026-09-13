using Microsoft.AspNetCore.Mvc;
using SmartBed.Data;
using SmartBed.Models;
using System.Linq;

namespace SmartBed.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // GET: Booking/Create
        // ==========================================

        public IActionResult Create(int hospitalId)
        {
            // Check whether user is logged in
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Find selected hospital
            var hospital = _context.Hospital
     .FirstOrDefault(h =>
         h.HospitalId == hospitalId &&
         h.VerificationStatus == "Verified");
            if (hospital == null)
            {
                return NotFound();
            }

            ViewBag.Hospital = hospital;

            return View();
        }


        // ==========================================
        // POST: Booking/Create
        // ==========================================

        [HttpPost]
        public IActionResult Create(
            int hospitalId,
            string patientName,
            int patientAge,
            string phone,
            string bedType)
        {
            // ==========================================
            // CHECK LOGIN
            // ==========================================

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }


            // ==========================================
            // FIND HOSPITAL
            // ==========================================

            var hospital = _context.Hospital
                .FirstOrDefault(h => h.HospitalId == hospitalId);

            if (hospital == null)
            {
                return NotFound();
            }


            // ==========================================
            // VALIDATE PATIENT NAME
            // ==========================================

            if (string.IsNullOrWhiteSpace(patientName))
            {
                ViewBag.Message = "Patient name is required.";
                ViewBag.Hospital = hospital;
                return View();
            }

            patientName = patientName.Trim();

            if (patientName.Length > 100)
            {
                ViewBag.Message = "Patient name cannot exceed 100 characters.";
                ViewBag.Hospital = hospital;
                return View();
            }

            if (!patientName.All(c => char.IsLetter(c) || c == ' '))
            {
                ViewBag.Message =
                    "Patient name should contain letters and spaces only.";

                ViewBag.Hospital = hospital;
                return View();
            }


            // ==========================================
            // VALIDATE PATIENT AGE
            // ==========================================

            if (patientAge < 1 || patientAge > 120)
            {
                ViewBag.Message =
                    "Patient age must be between 1 and 120.";

                ViewBag.Hospital = hospital;
                return View();
            }


            // ==========================================
            // VALIDATE PHONE NUMBER
            // ==========================================

            if (string.IsNullOrWhiteSpace(phone))
            {
                ViewBag.Message = "Contact number is required.";
                ViewBag.Hospital = hospital;
                return View();
            }

            phone = phone.Trim();

            if (phone.Length != 10 || !phone.All(char.IsDigit))
            {
                ViewBag.Message =
                    "Contact number must contain exactly 10 digits.";

                ViewBag.Hospital = hospital;
                return View();
            }


            // ==========================================
            // VALIDATE BED TYPE
            // ==========================================

            if (bedType != "ICU" &&
                bedType != "Emergency" &&
                bedType != "General")
            {
                ViewBag.Message =
                    "Please select a valid bed type.";

                ViewBag.Hospital = hospital;
                return View();
            }


            // ==========================================
            // CHECK DUPLICATE BOOKING
            // ==========================================

            var existingBooking = _context.Bookings
                .FirstOrDefault(b =>
                    b.UserId == userId.Value &&
                    b.HospitalId == hospitalId &&
                    b.Status == "Confirmed");

            if (existingBooking != null)
            {
                ViewBag.Message =
                    "You already have a confirmed booking at this hospital.";

                ViewBag.Hospital = hospital;

                return View();
            }


            // ==========================================
            // CHECK BED AVAILABILITY
            // ==========================================

            if (bedType == "ICU" && hospital.ICUBeds <= 0)
            {
                ViewBag.Message =
                    "Sorry, ICU beds are currently unavailable.";

                ViewBag.Hospital = hospital;

                return View();
            }

            if (bedType == "Emergency" &&
                hospital.EmergencyBeds <= 0)
            {
                ViewBag.Message =
                    "Sorry, Emergency beds are currently unavailable.";

                ViewBag.Hospital = hospital;

                return View();
            }

            if (bedType == "General" &&
                hospital.GeneralBeds <= 0)
            {
                ViewBag.Message =
                    "Sorry, General beds are currently unavailable.";

                ViewBag.Hospital = hospital;

                return View();
            }


            // ==========================================
            // CREATE BOOKING
            // ==========================================

            var booking = new Booking
            {
                UserId = userId.Value,
                HospitalId = hospitalId,
                PatientName = patientName,
                PatientAge = patientAge,
                Phone = phone,
                BedType = bedType,
                BookingDate = DateTime.Now,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);


            // ==========================================
            // DECREASE BED COUNT
            // ==========================================

            if (bedType == "ICU")
            {
                hospital.ICUBeds--;
            }
            else if (bedType == "Emergency")
            {
                hospital.EmergencyBeds--;
            }
            else if (bedType == "General")
            {
                hospital.GeneralBeds--;
            }


            // ==========================================
            // SAVE
            // ==========================================

            _context.SaveChanges();


            // ==========================================
            // REDIRECT TO CONFIRMATION
            // ==========================================

            return RedirectToAction(
                "Confirmation",
                new
                {
                    id = booking.BookingId
                }
            );
        }


        // ==========================================
        // BOOKING CONFIRMATION
        // ==========================================

        public IActionResult Confirmation(int id)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var booking = _context.Bookings
                .FirstOrDefault(b =>
                    b.BookingId == id &&
                    b.UserId == userId.Value);

            if (booking == null)
            {
                return NotFound();
            }

            var hospital = _context.Hospital
                .FirstOrDefault(h =>
                    h.HospitalId == booking.HospitalId);

            ViewBag.Hospital = hospital;

            return View(booking);
        }
        public IActionResult Rate(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var booking = _context.Bookings
                .FirstOrDefault(b =>
                    b.BookingId == id &&
                    b.UserId == userId.Value &&
                    b.Status == "Confirmed");

            if (booking == null)
            {
                return NotFound();
            }

            var hospital = _context.Hospital
                .FirstOrDefault(h => h.HospitalId == booking.HospitalId);

            if (hospital == null)
            {
                return NotFound();
            }

            ViewBag.Hospital = hospital;
            ViewBag.BookingId = booking.BookingId;

            return View();
        }
        [HttpPost]
        public IActionResult Rate(
    int bookingId,
    int rating,
    string comment)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (rating < 1 || rating > 5)
            {
                ViewBag.Message = "Please select a rating between 1 and 5.";

                var bookingForError = _context.Bookings
                    .FirstOrDefault(b =>
                        b.BookingId == bookingId &&
                        b.UserId == userId.Value);

                if (bookingForError != null)
                {
                    ViewBag.Hospital = _context.Hospital
                        .FirstOrDefault(h =>
                            h.HospitalId == bookingForError.HospitalId);
                }

                return View();
            }

            var booking = _context.Bookings
                .FirstOrDefault(b =>
                    b.BookingId == bookingId &&
                    b.UserId == userId.Value &&
                    b.Status == "Confirmed");

            if (booking == null)
            {
                return NotFound();
            }

            // Prevent the same user from rating the same booking twice
            var existingRating = _context.HospitalRatings
                .FirstOrDefault(r =>
                    r.UserId == userId.Value &&
                    r.HospitalId == booking.HospitalId);

            if (existingRating != null)
            {
                ViewBag.Message = "You have already rated this hospital.";

                ViewBag.Hospital = _context.Hospital
                    .FirstOrDefault(h =>
                        h.HospitalId == booking.HospitalId);

                return View();
            }

            var hospital = _context.Hospital
                .FirstOrDefault(h =>
                    h.HospitalId == booking.HospitalId);

            if (hospital == null)
            {
                return NotFound();
            }

            var hospitalRating = new HospitalRating
            {
                HospitalId = hospital.HospitalId,
                UserId = userId.Value,
                Rating = rating,
                Comment = comment ?? string.Empty,
                RatingDate = DateTime.Now
            };

            _context.HospitalRatings.Add(hospitalRating);

            _context.SaveChanges();

            TempData["RatingSuccess"] =
                "Thank you! Your rating has been submitted successfully.";

            return RedirectToAction(
                "Confirmation",
                new { id = booking.BookingId });
        }
    }
}
