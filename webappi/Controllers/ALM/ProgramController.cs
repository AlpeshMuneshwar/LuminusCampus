using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webappi.Data;

namespace webappi.Controllers.ALM
{
    [Authorize]
    public class ProgramController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgramController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Program
        public async Task<IActionResult> Index()
        {
            return View("~/Views/ALM/Program/Index.cshtml", await _context.ErpPrograms.ToListAsync());
        }

        // GET: Program/Create
        public IActionResult Create()
        {
            return View("~/Views/ALM/Program/Create.cshtml");
        }

        // POST: Program/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,DurationInYears,IsActive")] ErpProgram program)
        {
            if (ModelState.IsValid)
            {
                _context.Add(program);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/ALM/Program/Create.cshtml", program);
        }

        // GET: Program/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.ErpPrograms.FindAsync(id);
            if (program == null) return NotFound();
            return View("~/Views/ALM/Program/Edit.cshtml", program);
        }

        // POST: Program/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,DurationInYears,IsActive,CreatedOn,CreatedBy")] ErpProgram program)
        {
            if (id != program.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(program);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ErpPrograms.Any(e => e.Id == program.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/ALM/Program/Edit.cshtml", program);
        }

        // GET: Program/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
             if (id == null) return NotFound();
             var program = await _context.ErpPrograms.FindAsync(id);
             if (program != null)
             {
                 _context.ErpPrograms.Remove(program);
                 await _context.SaveChangesAsync();
             }
             return RedirectToAction(nameof(Index));
        }
    }
}
