using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webappi.Data;

namespace webappi.Controllers.ALM
{
    [Authorize]
    public class BranchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Branch
        public async Task<IActionResult> Index()
        {
            var branches = _context.ErpBranches.Include(b => b.Program).Include(b => b.Department);
            return View("~/Views/ALM/Branch/Index.cshtml", await branches.ToListAsync());
        }

        // GET: Branch/Create
        public IActionResult Create()
        {
            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments.Where(d => d.IsActive), "Id", "Name");
            return View("~/Views/ALM/Branch/Create.cshtml");
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,ProgramId,DepartmentId,IsActive")] ErpBranch branch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name", branch.ProgramId);
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments.Where(d => d.IsActive), "Id", "Name", branch.DepartmentId);
            return View("~/Views/ALM/Branch/Create.cshtml", branch);
        }

        // GET: Branch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var branch = await _context.ErpBranches.FindAsync(id);
            if (branch == null) return NotFound();

            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name", branch.ProgramId);
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments.Where(d => d.IsActive), "Id", "Name", branch.DepartmentId);
            return View("~/Views/ALM/Branch/Edit.cshtml", branch);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,ProgramId,DepartmentId,IsActive,CreatedOn,CreatedBy")] ErpBranch branch)
        {
            if (id != branch.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ErpBranches.Any(e => e.Id == branch.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProgramId"] = new SelectList(_context.ErpPrograms.Where(p => p.IsActive), "Id", "Name", branch.ProgramId);
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments.Where(d => d.IsActive), "Id", "Name", branch.DepartmentId);
            return View("~/Views/ALM/Branch/Edit.cshtml", branch);
        }

        // GET: Branch/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
             if (id == null) return NotFound();
             var branch = await _context.ErpBranches.FindAsync(id);
             if (branch != null)
             {
                 _context.ErpBranches.Remove(branch);
                 await _context.SaveChangesAsync();
             }
             return RedirectToAction(nameof(Index));
        }
    }
}
