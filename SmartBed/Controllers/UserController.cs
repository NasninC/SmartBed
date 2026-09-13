using Microsoft.AspNetCore.Mvc;
using SmartBed.Data;
using System.Linq;

namespace SmartBed.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard(string search)
        {
            var hospitals = _context.Hospital
                .Where(h => h.VerificationStatus == "Verified")
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                hospitals = hospitals.Where(h =>
                    h.HospitalName.Contains(search) ||
                    h.Location.Contains(search));
            }

            var hospitalList = hospitals.ToList();

            ViewBag.Ratings = _context.HospitalRatings
                .GroupBy(r => r.HospitalId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Average = g.Average(r => r.Rating),
                        Count = g.Count()
                    });

            return View(hospitalList);
        }
    }
}
