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
            var hospitals = _context.Hospital.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                hospitals = hospitals.Where(h =>
                    h.HospitalName.Contains(search) ||
                    h.Location.Contains(search));
            }

            return View(hospitals.ToList());
        }
    }
}
