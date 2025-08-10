using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.PL.Controllers
{
    public class AgentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AgentsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: Index
        public async Task<IActionResult> Index(string search)
        {
            var agents = await _unitOfWork.Agents.GetAllAsync(includeProperties: "Project,TeamLeader");

            if (!string.IsNullOrEmpty(search))
            {
                string loweredSearch = search.ToLower();
                agents = agents.Where(a =>
                    (!string.IsNullOrEmpty(a.HRID) && a.HRID.ToLower().Contains(loweredSearch)) ||
                    (!string.IsNullOrEmpty(a.Name) && a.Name.ToLower().Contains(loweredSearch)) ||
                    (!string.IsNullOrEmpty(a.LoginID) && a.LoginID.ToLower().Contains(loweredSearch)) ||
                    (!string.IsNullOrEmpty(a.NTName) && a.NTName.ToLower().Contains(loweredSearch))
                ).ToList();
            }

            var agentDtos = _mapper.Map<IEnumerable<AgentReadDto>>(agents);
            ViewBag.Search = search;
            return View(agentDtos);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (agent == null) return NotFound();
            var agentDto = _mapper.Map<AgentDetailsDto>(agent); // ✅ تحويل إلى DTO
            return View(agentDto);
        }


        // GET: Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAgentDto createAgentDto)
        {
            if (ModelState.IsValid)
            {
                var agent = _mapper.Map<Agent>(createAgentDto);
                await _unitOfWork.Agents.AddAsync(agent);
                var rows = await _unitOfWork.CompleteAsync();

                if (rows > 0)
                    return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns();
            return View(createAgentDto);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (agent == null) return NotFound();

            var editDto = _mapper.Map<EditAgentDto>(agent);
            await PopulateDropdowns();
            return View(editDto);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAgentDto editAgentDto)
        {
            if (id != editAgentDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var agent = _mapper.Map<Agent>(editAgentDto);
                _unitOfWork.Agents.Update(agent);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns();
            return View(editAgentDto);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (agent == null) return NotFound();

            var agentDto = _mapper.Map<AgentDetailsDto>(agent); // 
            return View(agentDto);
        }

        // POST: Delete Confirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);

            // تحقق لو مرتبط كـ TeamLeader
            var subAgents = await _unitOfWork.Agents.FindAsync(a => a.TeamLeaderId == id);
            if (subAgents.Any())
            {
                ModelState.AddModelError("", "⚠️ This agent cannot be deleted because they are assigned as a Team Leader to other agents.");
                var agentDto = _mapper.Map<AgentDetailsDto>(agent);
                return View("Delete", agentDto);
            }

            _unitOfWork.Agents.Delete(agent);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }


        private async Task PopulateDropdowns()
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var teamLeaders = await _unitOfWork.Agents.GetAllAsync();

            ViewBag.Projects = new SelectList(projects, "Id", "Name");
            ViewBag.TeamLeaders = new SelectList(teamLeaders, "Id", "Name");
        }
    }
}
