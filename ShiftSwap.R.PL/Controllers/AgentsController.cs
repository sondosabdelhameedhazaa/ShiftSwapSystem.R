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

        public async Task<IActionResult> Index()
        {
            var ntName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(ntName))
                return RedirectToAction("Login", "Account");

            var allAgents = await _unitOfWork.Agents.GetAllAsync(includeProperties: "Project,TeamLeader");

            var currentAgent = allAgents.FirstOrDefault(a => a.NTName.ToLower() == ntName.ToLower());

            if (currentAgent == null)
                return RedirectToAction("Login", "Account");

            var agentsInSameProject = allAgents
                .Where(a => a.ProjectId == currentAgent.ProjectId && a.Id != currentAgent.Id)
                .ToList();

            var agentDtos = _mapper.Map<IEnumerable<AgentReadDto>>(agentsInSameProject);
            return View(agentDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (agent == null) return NotFound();
            var agentDto = _mapper.Map<AgentDetailsDto>(agent);
            return View(agentDto);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (agent == null) return NotFound();

            var editDto = _mapper.Map<EditAgentDto>(agent);
            await PopulateDropdowns();
            return View(editDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAgentDto editAgentDto)
        {
            if (id != editAgentDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var agent = _mapper.Map<Agent>(editAgentDto);
                await _unitOfWork.Agents.UpdateAsync(agent); // ✅ تم التعديل هنا
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns();
            return View(editAgentDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);
            if (agent == null) return NotFound();

            var agentDto = _mapper.Map<AgentDetailsDto>(agent);
            return View(agentDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agent = await _unitOfWork.Agents.GetByIdAsync(id);

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
