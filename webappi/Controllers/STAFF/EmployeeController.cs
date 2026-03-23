using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webappi.Data;

namespace webappi.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeeController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var employees = _context.ErpEmployees.Include(e => e.Department).Include(e => e.Designation);
            return View(await employees.ToListAsync());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.ErpEmployees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.ErpDesignations, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ErpEmployee employee, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(Photo.FileName);
                    string extension = Path.GetExtension(Photo.FileName);
                    employee.PhotoPath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/employees/", fileName);
                    
                    // Creates directory if not exists
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments, "Id", "Name", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.ErpDesignations, "Id", "Name", employee.DesignationId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", employee.UserId);
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.ErpEmployees.FindAsync(id);
            if (employee == null) return NotFound();
            
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments, "Id", "Name", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.ErpDesignations, "Id", "Name", employee.DesignationId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", employee.UserId);
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ErpEmployee employee, IFormFile? Photo)
        {
            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.ErpEmployees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    
                    if (Photo != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(Photo.FileName);
                        string extension = Path.GetExtension(Photo.FileName);
                        string newFileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/images/employees/", newFileName);
                        
                         Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await Photo.CopyToAsync(fileStream);
                        }
                        employee.PhotoPath = newFileName;
                    }
                    else
                    {
                        employee.PhotoPath = existing?.PhotoPath;
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.ErpDepartments, "Id", "Name", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.ErpDesignations, "Id", "Name", employee.DesignationId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", employee.UserId);
            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.ErpEmployees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.ErpEmployees.FindAsync(id);
            if (employee != null)
            {
                _context.ErpEmployees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.ErpEmployees.Any(e => e.Id == id);
        }
    }
}
