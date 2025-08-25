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
        private readonly IMapper _mapper;

        public ShiftSwapRequestController(
            IShiftSwapRequestRepository shiftSwapRepo,
            IAgentRepository agentRepo,
            IMapper mapper)
        {
            _shiftSwapRepo = shiftSwapRepo;
            _agentRepo = agentRepo;
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

        public async Task<IActionResult> Create(int? targetAgentId)
        {
            var ntName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(ntName))
                return Unauthorized("User not authenticated.");

            var currentAgent = await _agentRepo.GetByNTNameAsync(ntName);
            if (currentAgent == null)
                return Unauthorized("Agent not found in the system.");

            var agentsInSameProject = await _agentRepo.GetAgentsByProjectAsync(currentAgent.ProjectId);
            var targetAgents = agentsInSameProject
                .Where(a => a.Id != currentAgent.Id)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

            if (!targetAgents.Any())
                TempData["Error"] = "No available agents in your project to swap with.";

            ViewBag.TargetAgents = targetAgents;

            var createDto = new ShiftSwapRequestCreateDto
            {
                TargetAgentId = targetAgentId ?? 0
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
            {
                ModelState.AddModelError("", "Agent not found in the system.");
                return await LoadFormAgain(createDto, null);
            }

            var target = await _agentRepo.GetByIdAsync(createDto.TargetAgentId);
            if (target == null)
            {
                ModelState.AddModelError("", "Target agent not found.");
                return await LoadFormAgain(createDto, requestor.Id);
            }

            if (requestor.ProjectId != target.ProjectId)
            {
                ModelState.AddModelError("", "Swap requests must be within the same project.");
                return await LoadFormAgain(createDto, requestor.Id);
            }

            if (createDto.SwapDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("", "You cannot create a swap request for a past date.");
                return await LoadFormAgain(createDto, requestor.Id);
            }

            if (createDto.SwapDate.Date == DateTime.Now.Date)
            {
                ModelState.AddModelError("", "You cannot create a swap request for today.");
                return await LoadFormAgain(createDto, requestor.Id);
            }

            if (!ModelState.IsValid)
                return await LoadFormAgain(createDto, requestor.Id);

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

        private async Task<IActionResult> LoadFormAgain(ShiftSwapRequestCreateDto createDto, int? requestorId)
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
                        Text = a.Name
                    }).ToList();

                ViewBag.TargetAgents = targetAgents;
            }
            else
            {
                ViewBag.TargetAgents = new List<SelectListItem>();
            }

            return View("Create", createDto);
        }
    }
}

