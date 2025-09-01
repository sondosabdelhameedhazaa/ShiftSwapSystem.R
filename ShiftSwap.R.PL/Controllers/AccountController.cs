using Microsoft.AspNetCore.Mvc;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;
using ShiftSwap.R.DAL.Models.Enums;
using ShiftSwap.R.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShiftSwap.R.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAgentRepository _agentRepo;

        public AccountController(IAgentRepository agentRepo)
        {
            _agentRepo = agentRepo;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                // Save session data
                HttpContext.Session.SetString("UserRole", agent.Role.ToString());
                HttpContext.Session.SetString("UserProject", agent.Project?.Name ?? "");
                HttpContext.Session.SetString("UserName", agent.NTName ?? agent.Name);
                HttpContext.Session.SetString("LoginID", agent.LoginID);

                // Redirect based on role
                return agent.Role switch
                {
                    AgentRole.Agent => RedirectToAction("MySchedule", "ShiftSchedule"),
                    AgentRole.TeamLeader => RedirectToAction("Pending", "ShiftSwapRequest"),
                    AgentRole.RTM => RedirectToAction("Pending", "ShiftSwapRequest"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            // Invalid login
            ModelState.AddModelError("", "Invalid credentials. Please check your Login ID and Name.");
            return View(loginDto);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
