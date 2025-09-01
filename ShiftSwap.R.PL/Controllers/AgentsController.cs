using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.DAL.Models.Enums;
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

        public async Task<IActionResult> Index(DateTime? date, string shiftFrom, string shiftTo)
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return RedirectToAction("Login", "Account");

            var currentAgent = await _unitOfWork.Agents.GetByNTNameAsync(ntName);
            if (currentAgent == null)
                return RedirectToAction("Login", "Account");

            ViewBag.IsAgent = currentAgent.Role == AgentRole.Agent;
            ViewBag.SearchDate = date?.ToString("yyyy-MM-dd");
            ViewBag.SearchShiftFrom = shiftFrom;
            ViewBag.SearchShiftTo = shiftTo;

            if (!date.HasValue)
                return View(new List<AgentReadDto>());

            // تحويل الوقت من string لـ TimeSpan
            // بدل القيمة الافتراضية، نخليها nullable
            TimeSpan? shiftFromTime = TimeSpan.TryParse(shiftFrom, out var tempFrom) ? tempFrom : (TimeSpan?)null;
            TimeSpan? shiftToTime = TimeSpan.TryParse(shiftTo, out var tempTo) ? tempTo : (TimeSpan?)null;

            // Get agents with shifts on that day excluding the current agent
            var availableAgentsWithShifts = await _unitOfWork.Agents
                .GetAvailableAgentsWithShiftsAsync(date.Value, currentAgent.Id);

            // Filter by shift times
            availableAgentsWithShifts = availableAgentsWithShifts
    .Where(a => (!shiftFromTime.HasValue || a.Schedule.ShiftStart >= shiftFromTime.Value)
             && (!shiftToTime.HasValue || a.Schedule.ShiftEnd <= shiftToTime.Value))
    .ToList();


            // Filter only agents in the same project if current user is not an Agent
            if (currentAgent.Role != AgentRole.Agent)
            {
                availableAgentsWithShifts = availableAgentsWithShifts
                    .Where(x => x.Agent.ProjectId == currentAgent.ProjectId)
                    .ToList();
            }

            var agentDtos = availableAgentsWithShifts.Select(tuple => new AgentReadDto
            {
                Id = tuple.Agent.Id,
                Name = tuple.Agent.Name,
                HRID = tuple.Agent.HRID,
                LoginID = tuple.Agent.LoginID,
                NTName = tuple.Agent.NTName,
                Role = tuple.Agent.Role,
                ProjectName = tuple.Agent.Project?.Name,
                TeamLeaderName = tuple.Agent.TeamLeader?.Name,
                ShiftStart = tuple.Schedule.ShiftStart,
                ShiftEnd = tuple.Schedule.ShiftEnd,
                Shift = tuple.Schedule.Shift
            }).ToList();

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
                await _unitOfWork.Agents.UpdateAsync(agent);
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

