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

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            var identifier = loginDto.Identifier?.Trim();
            Agent agent = loginDto.IdentifierType switch
            {
                "HRID" => await _agentRepo.GetByHRIDAsync(identifier),
                "NTName" => await _agentRepo.GetByNTNameAsync(identifier),
                "LoginID" => await _agentRepo.GetByLoginIDAsync(identifier),
                _ => null
            };

            if (agent != null)
            {
                // Store user info in session
                HttpContext.Session.SetString("Identifier", identifier);
                HttpContext.Session.SetString("IdentifierType", loginDto.IdentifierType);
                HttpContext.Session.SetString("UserRole", agent.Role.ToString());
                HttpContext.Session.SetString("UserProject", agent.Project?.Name ?? "");
                HttpContext.Session.SetString("UserName", agent.NTName); 

                // Redirect based on role
                return agent.Role switch
                {
                    AgentRole.Agent => RedirectToAction("Index", "Agents"),
                    AgentRole.TeamLeader => RedirectToAction("Pending", "ShiftSwapRequest"),
                    AgentRole.RTM => RedirectToAction("Pending", "ShiftSwapRequest"),
                    _ => RedirectToAction("Index", "Home")
                };
            }


            ModelState.AddModelError("", "Invalid credentials");
            return View(loginDto);
        }
    }
}
