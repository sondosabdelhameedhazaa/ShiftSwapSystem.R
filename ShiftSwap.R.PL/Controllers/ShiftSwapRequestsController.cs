using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.DAL.Models.Enums;
using ShiftSwap.R.PL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ShiftSwap.R.PL.Controllers
{
    public class ShiftSwapRequestController : Controller
    {
        private readonly IShiftSwapRequestRepository _shiftSwapRepo;
        private readonly IAgentRepository _agentRepo;
        private readonly IShiftScheduleRepository _shiftScheduleRepo;
        private readonly IMapper _mapper;

        public ShiftSwapRequestController(
            IShiftSwapRequestRepository shiftSwapRepo,
            IAgentRepository agentRepo,
            IShiftScheduleRepository shiftScheduleRepo,
            IMapper mapper)
        {
            _shiftSwapRepo = shiftSwapRepo;
            _agentRepo = agentRepo;
            _shiftScheduleRepo = shiftScheduleRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> Pending()
        {
            var requests = await _shiftSwapRepo.GetPendingRequestsAsync();
            var dto = _mapper.Map<IEnumerable<ShiftSwapRequestReadDto>>(requests);
            return View(dto);
        }

        public async Task<IActionResult> MyRequests()
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Unauthorized("User not authenticated.");

            var loggedInAgent = await _agentRepo.GetByNTNameAsync(ntName);
            if (loggedInAgent == null)
                return Unauthorized("User not found in the system.");

            var requests = await _shiftSwapRepo.GetByAgentIdAsync(loggedInAgent.Id);
            var dto = _mapper.Map<IEnumerable<ShiftSwapRequestReadDto>>(requests);
            return View(dto);
        }

        // GET: Create Swap Request for agents (regular)
        public async Task<IActionResult> Create(int targetAgentId, DateTime? swapDate)
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Unauthorized("User not authenticated.");

            var currentAgent = await _agentRepo.GetByNTNameAsync(ntName);
            if (currentAgent == null)
                return Unauthorized("Agent not found in the system.");

            var targetAgent = await _agentRepo.GetByIdAsync(targetAgentId);
            if (targetAgent == null)
                return NotFound("Target agent not found.");

            ViewBag.TargetAgents = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = targetAgent.Id.ToString(),
                    Text = targetAgent.Name,
                    Selected = true
                }
            };

            var selectedDate = swapDate ?? DateTime.Today;

            var yourShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(currentAgent.Id, selectedDate, selectedDate)).FirstOrDefault();
            var targetShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(targetAgent.Id, selectedDate, selectedDate)).FirstOrDefault();

            ViewBag.YourShift = yourShift != null ? $"{yourShift.ShiftStart:hh\\:mm} - {yourShift.ShiftEnd:hh\\:mm}" : "";
            ViewBag.TargetShift = targetShift != null ? $"{targetShift.ShiftStart:hh\\:mm} - {targetShift.ShiftEnd:hh\\:mm}" : "";

            var createDto = new ShiftSwapRequestCreateDto
            {
                TargetAgentId = targetAgent.Id,
                SwapDate = selectedDate
            };

            return View(createDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShiftSwapRequestCreateDto createDto)
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Unauthorized("User not authenticated.");

            var requestor = await _agentRepo.GetByNTNameAsync(ntName);
            if (requestor == null)
                return await LoadFormAgain(createDto, null, "Agent not found in the system.");

            var target = await _agentRepo.GetByIdAsync(createDto.TargetAgentId);
            if (target == null)
                return await LoadFormAgain(createDto, requestor.Id, "Target agent not found.");

            if (requestor.ProjectId != target.ProjectId)
                return await LoadFormAgain(createDto, requestor.Id, "Swap requests must be within the same project.");

            if (createDto.SwapDate.Date <= DateTime.Now.Date)
                return await LoadFormAgain(createDto, requestor.Id, "Swap date must be in the future.");

            var existingSwaps = await _shiftSwapRepo.GetByDateAsync(createDto.SwapDate.Date);
            var agentsInPendingSwaps = existingSwaps
                .Where(r => r.Status == SwapStatus.Pending)
                .SelectMany(r => new[] { r.RequestorAgentId, r.TargetAgentId })
                .Distinct()
                .ToList();

            if (agentsInPendingSwaps.Contains(requestor.Id) || agentsInPendingSwaps.Contains(createDto.TargetAgentId))
            {
                return await LoadFormAgain(createDto, requestor.Id, "Please find another agent");
            }

            var swapRequest = _mapper.Map<ShiftSwapRequest>(createDto);
            swapRequest.RequestorAgentId = requestor.Id;
            swapRequest.TargetAgentId = target.Id;
            swapRequest.Status = SwapStatus.Pending;

            await _shiftSwapRepo.AddAsync(swapRequest);

            TempData["Success"] = "Swap request created successfully.";
            return RedirectToAction(nameof(MyRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOrReject(ShiftSwapRequestApprovalDto approvalDto)
        {
            var request = await _shiftSwapRepo.GetByIdAsync(approvalDto.RequestId);
            if (request == null)
                return NotFound("Swap request not found.");

            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Unauthorized("User not authenticated.");

            var approver = await _agentRepo.GetByNTNameAsync(ntName);
            if (approver == null)
                return Unauthorized("Approver not found.");

            request.Status = approvalDto.IsApproved ? SwapStatus.Approved : SwapStatus.Rejected;
            request.Comment = approvalDto.Comment ?? "";
            request.ApprovedById = approver.Id;

            await _shiftSwapRepo.UpdateAsync(request);

            TempData["Success"] = approvalDto.IsApproved
                ? "Swap request approved successfully."
                : "Swap request rejected.";

            return RedirectToAction(nameof(Pending));
        }

        // ===== TL Direct Swap =====
        [HttpGet]
        public async Task<IActionResult> TLDirectSwap()
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Unauthorized();

            var tl = await _agentRepo.GetByNTNameAsync(ntName);
            if (tl == null || tl.Role != AgentRole.TeamLeader)
                return Unauthorized();

            // جلب كل الـ agents اللي في نفس المشروع واللي Role بتاعهم Agent فقط
            var teamAgents = (await _agentRepo.GetAllAsync())
     .Where(a => a.Role == AgentRole.Agent)
     .OrderBy(a => a.Name)
     .Select(a => new SelectListItem
     {
         Value = a.Id.ToString(),
         Text = $"{a.Name} ({a.HRID})"
     }).ToList();

            ViewBag.TeamAgents = teamAgents;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TLDirectSwap(int requestorId, int targetId, DateTime swapDate)
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Unauthorized();

            var tl = await _agentRepo.GetByNTNameAsync(ntName);
            if (tl == null || tl.Role != AgentRole.TeamLeader)
                return Unauthorized();

            //var teamAgents = await _agentRepo.GetAgentsByProjectAsync(tl.ProjectId);
            //if (!teamAgents.Any(a => a.Id == requestorId || a.Id == targetId))
            //    return Unauthorized("Agents not in your team.");

            var requestorShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(requestorId, swapDate, swapDate)).FirstOrDefault();
            var targetShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(targetId, swapDate, swapDate)).FirstOrDefault();

            if (requestorShift == null || targetShift == null)
                return BadRequest("Shifts not found for given date.");

            // Swap shift times
            var tempStart = requestorShift.ShiftStart;
            var tempEnd = requestorShift.ShiftEnd;

            requestorShift.ShiftStart = targetShift.ShiftStart;
            requestorShift.ShiftEnd = targetShift.ShiftEnd;

            targetShift.ShiftStart = tempStart;
            targetShift.ShiftEnd = tempEnd;

            await _shiftScheduleRepo.UpdateAsync(requestorShift);
            await _shiftScheduleRepo.UpdateAsync(targetShift);

            // سجل swap request جديد
            var swapRequest = new ShiftSwapRequest
            {
                RequestorAgentId = requestorId,
                TargetAgentId = targetId,
                SwapDate = swapDate,
                Status = SwapStatus.Pending, // يظهر في Pending
                Comment = $"Direct swap by Team Leader {tl.NTName}",
                CreatedAt = DateTime.Now
            };
            await _shiftSwapRepo.AddAsync(swapRequest);

            TempData["Success"] = "Shift swapped successfully!";
            return RedirectToAction("Pending");
        }


        private async Task<IActionResult> LoadFormAgain(ShiftSwapRequestCreateDto createDto, int? requestorId, string? errorMessage = null)
        {
            if (requestorId.HasValue)
            {
                var agents = await _agentRepo.GetAgentsByProjectAsync(
                    (await _agentRepo.GetByIdAsync(requestorId.Value)).ProjectId);

                var targetAgents = agents
                    .Where(a => a.Id != requestorId.Value)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name,
                        Selected = (a.Id == createDto.TargetAgentId)
                    }).ToList();

                ViewBag.TargetAgents = targetAgents;
            }
            else
            {
                ViewBag.TargetAgents = new List<SelectListItem>();
            }

            if (!string.IsNullOrEmpty(errorMessage))
                ModelState.AddModelError("", errorMessage);

            var today = createDto.SwapDate.Date;
            var currentAgent = await _agentRepo.GetByNTNameAsync(HttpContext.Session.GetString("UserName"));
            if (currentAgent != null)
            {
                var yourShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(currentAgent.Id, today, today)).FirstOrDefault();
                ViewBag.YourShift = yourShift != null
                    ? $"{yourShift.ShiftStart:hh\\:mm} - {yourShift.ShiftEnd:hh\\:mm}"
                    : "";
            }

            if (createDto.TargetAgentId > 0)
            {
                var targetShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(createDto.TargetAgentId, today, today)).FirstOrDefault();
                ViewBag.TargetShift = targetShift != null
                    ? $"{targetShift.ShiftStart:hh\\:mm} - {targetShift.ShiftEnd:hh\\:mm}"
                    : "";
            }
            else
            {
                ViewBag.TargetShift = "";
            }

            return View("Create", createDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetShifts(int targetAgentId, DateTime swapDate)
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Json(new { yourShift = "", targetShift = "" });

            var currentAgent = await _agentRepo.GetByNTNameAsync(ntName);

            var yourShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(currentAgent.Id, swapDate, swapDate)).FirstOrDefault();
            var targetShift = (await _shiftScheduleRepo.GetSchedulesForAgentAsync(targetAgentId, swapDate, swapDate)).FirstOrDefault();

            return Json(new
            {
                yourShift = yourShift != null ? $"{yourShift.ShiftStart:hh\\:mm} - {yourShift.ShiftEnd:hh\\:mm}" : "",
                targetShift = targetShift != null ? $"{targetShift.ShiftStart:hh\\:mm} - {targetShift.ShiftEnd:hh\\:mm}" : ""
            });
        }
    }
}
