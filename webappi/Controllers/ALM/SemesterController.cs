using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webappi.Data;

namespace webappi.Controllers.ALM
{
    [Authorize]
    public class SemesterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SemesterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var semesters = _context.ErpSemesters.Include(s => s.Program).OrderBy(s => s.Program.Name).ThenBy(s => s.SequenceNo);
            return View("~/Views/ALM/Semester/Index.cshtml", await semesters.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name");
            return View("~/Views/ALM/Semester/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,SequenceNo,ProgramId,IsActive")] ErpSemester semester)
        {
            if (ModelState.IsValid)
            {
                _context.Add(semester);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name", semester.ProgramId);
            return View("~/Views/ALM/Semester/Create.cshtml", semester);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var semester = await _context.ErpSemesters.FindAsync(id);
            if (semester == null) return NotFound();
            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name", semester.ProgramId);
            return View("~/Views/ALM/Semester/Edit.cshtml", semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,SequenceNo,ProgramId,IsActive,CreatedOn,CreatedBy")] ErpSemester semester)
        {
            if (id != semester.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(semester);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ErpSemesters.Any(e => e.Id == semester.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name", semester.ProgramId);
            return View("~/Views/ALM/Semester/Edit.cshtml", semester);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
             if (id == null) return NotFound();
             var semester = await _context.ErpSemesters.FindAsync(id);
             if (semester != null)
             {
                 _context.ErpSemesters.Remove(semester);
                 await _context.SaveChangesAsync();
             }
             return RedirectToAction(nameof(Index));
        }
    }
}
