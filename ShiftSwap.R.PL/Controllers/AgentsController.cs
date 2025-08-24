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
            // 1. جيب الـ NTName من السيشن (اتسجل وقت اللوجين)
            var ntName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(ntName))
                return RedirectToAction("Login", "Account");

            // 2. جيب كل الـ Agents مع الـ Project والـ TL
            var allAgents = await _unitOfWork.Agents.GetAllAsync(includeProperties: "Project,TeamLeader");

            // 3. حدد مين هو المستخدم اللي عمل لوجين
            var currentAgent = allAgents.FirstOrDefault(a => a.NTName.ToLower() == ntName.ToLower());

            if (currentAgent == null)
                return RedirectToAction("Login", "Account");

            // 4. جيب الناس اللي معاه في نفس المشروع (واستبعد نفسه)
            var agentsInSameProject = allAgents
                .Where(a => a.ProjectId == currentAgent.ProjectId && a.Id != currentAgent.Id)
                .ToList();

            // 5. حولهم لـ DTOs واعرضهم
            var agentDtos = _mapper.Map<IEnumerable<AgentReadDto>>(agentsInSameProject);
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
