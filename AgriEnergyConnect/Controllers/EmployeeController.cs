using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgriEnergyConnect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AllFarmers()
        {
            var farmers = await _context.Farmers.ToListAsync();
            return View(farmers);
        }

        [HttpGet]
        public IActionResult AddFarmer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                _context.Farmers.Add(farmer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Farmer added successfully!";
                return RedirectToAction(nameof(AllFarmers));
            }

            return View(farmer);
        }

        [HttpGet]
        public async Task<IActionResult> ViewProducts(int farmerId, string category, DateTime? startDate, DateTime? endDate)
        {
            var products = _context.Products
                .Include(p => p.Farmer)
                .Where(p => p.FarmerId == farmerId);

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);

            if (startDate.HasValue)
                products = products.Where(p => p.ProductionDate >= startDate);

            if (endDate.HasValue)
                products = products.Where(p => p.ProductionDate <= endDate);

            return View(await products.ToListAsync());
        }
    }
}
