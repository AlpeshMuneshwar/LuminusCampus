using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webappi.Data;

namespace webappi.Controllers.ALM
{
    [Authorize]
    public class AcademicYearController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicYearController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View("~/Views/ALM/AcademicYear/Index.cshtml", await _context.ErpAcademicYears.OrderByDescending(y => y.StartDate).ToListAsync());
        }

        public IActionResult Create()
        {
            return View("~/Views/ALM/AcademicYear/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StartDate,EndDate,IsCurrent,IsActive")] ErpAcademicYear academicYear)
        {
            if (ModelState.IsValid)
            {
                if (academicYear.IsCurrent)
                {
                    // Unset IsCurrent for other years
                    await _context.Database.ExecuteSqlRawAsync("UPDATE ErpAcademicYears SET IsCurrent = 0");
                }
                _context.Add(academicYear);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/ALM/AcademicYear/Create.cshtml", academicYear);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var academicYear = await _context.ErpAcademicYears.FindAsync(id);
            if (academicYear == null) return NotFound();
            return View("~/Views/ALM/AcademicYear/Edit.cshtml", academicYear);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,IsCurrent,IsActive,CreatedOn,CreatedBy")] ErpAcademicYear academicYear)
        {
            if (id != academicYear.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (academicYear.IsCurrent)
                    {
                         await _context.Database.ExecuteSqlRawAsync("UPDATE ErpAcademicYears SET IsCurrent = 0 WHERE Id != {0}", id);
                    }
                    _context.Update(academicYear);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ErpAcademicYears.Any(e => e.Id == academicYear.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/ALM/AcademicYear/Edit.cshtml", academicYear);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
             if (id == null) return NotFound();
             var ay = await _context.ErpAcademicYears.FindAsync(id);
             if (ay != null)
             {
                 _context.ErpAcademicYears.Remove(ay);
                 await _context.SaveChangesAsync();
             }
             return RedirectToAction(nameof(Index));
        }
    }
}
