using Microsoft.AspNetCore.Mvc;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;
using ShiftSwap.R.DAL.Models.Enums;
using ShiftSwap.R.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace ShiftSwap.R.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAgentRepository _agentRepo;

        public AccountController(IAgentRepository agentRepo)
        {
            _agentRepo = agentRepo;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            var loginId = loginDto.LoginID?.Trim().ToLower();
            var name = loginDto.Name?.Trim().ToLower();

            var agent = await _agentRepo.FindFirstAsync(a =>
                a.LoginID.ToLower() == loginId &&
                a.Name.ToLower() == name
            );

            if (agent != null)
            {
                HttpContext.Session.SetString("UserRole", agent.Role.ToString());
                HttpContext.Session.SetString("UserProject", agent.Project?.Name ?? "");
                HttpContext.Session.SetString("UserName", agent.NTName ?? agent.Name);
                HttpContext.Session.SetString("LoginID", agent.LoginID);

                return agent.Role switch
                {
                    AgentRole.Agent => RedirectToAction("Index", "Agents"),
                    AgentRole.TeamLeader => RedirectToAction("Pending", "ShiftSwapRequest"),
                    AgentRole.RTM => RedirectToAction("Pending", "ShiftSwapRequest"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError("", "Invalid credentials. Please check your Login ID and Name.");
            return View(loginDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
