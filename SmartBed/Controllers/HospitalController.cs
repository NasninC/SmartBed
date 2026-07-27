using Microsoft.AspNetCore.Mvc;
using SmartBed.Data;
using SmartBed.Models;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using SmartBed.Hubs;
using Microsoft.AspNetCore.Http;

namespace SmartBed.Controllers
{
    public class HospitalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<BedHub> _hubContext;

        public HospitalController(ApplicationDbContext context,
                           IHubContext<BedHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult Dashboard()
        {
            int? hospitalId = HttpContext.Session.GetInt32("HospitalId");

            if (hospitalId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var hospital = _context.Hospital.FirstOrDefault(h => h.HospitalId == hospitalId);

            return View(hospital);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBeds(Hospital hospital)
        {
            var data = _context.Hospital.Find(hospital.HospitalId);

            if (data != null)
            {
                data.ICUBeds = hospital.ICUBeds;
                data.EmergencyBeds = hospital.EmergencyBeds;
                data.GeneralBeds = hospital.GeneralBeds;

                _context.SaveChanges();
                await _hubContext.Clients.All.SendAsync("ReceiveBedUpdate");
            }

            return RedirectToAction("Dashboard");
        }
    }
}