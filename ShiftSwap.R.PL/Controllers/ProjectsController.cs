using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ShiftSwapDbContext _context;
        private readonly IMapper _mapper;

        public ProjectsController(ShiftSwapDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                                         .Include(p => p.Agents)
                                         .ToListAsync();

            var projectDtos = _mapper.Map<IEnumerable<ProjectReadDto>>(projects);
            return View(projectDtos);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Agents)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null) return NotFound();

            var projectDto = _mapper.Map<ProjectDetailsDto>(project);
            return View(projectDto);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectDto createProjectDto)
        {
            if (ModelState.IsValid)
            {
                var project = _mapper.Map<Project>(createProjectDto);
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createProjectDto);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            var editProjectDto = _mapper.Map<EditProjectDto>(project);
            return View(editProjectDto);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProjectDto editProjectDto)
        {
            if (id != editProjectDto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var project = _mapper.Map<Project>(editProjectDto);
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projects.Any(e => e.Id == editProjectDto.Id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editProjectDto);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Agents)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null) return NotFound();

            var projectDto = _mapper.Map<ProjectDetailsDto>(project);
            return View(projectDto);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

