using Microsoft.AspNetCore.Mvc;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;
using ShiftSwap.R.DAL.Models.Enums;
using ShiftSwap.R.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftSwap.R.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAgentRepository _agentRepo;
        private readonly IProjectRepository _projectRepo;

        public AccountController(IAgentRepository agentRepo, IProjectRepository projectRepo)
        {
            _agentRepo = agentRepo;
            _projectRepo = projectRepo;
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

            var user = FakeUsers.Users.FirstOrDefault(u =>
                u.Type == loginDto.IdentifierType &&
                u.Identifier == loginDto.Identifier);

            if (user != default)
            {
                // تخزين بيانات المستخدم في الـ Session
                HttpContext.Session.SetString("Identifier", user.Identifier);
                HttpContext.Session.SetString("IdentifierType", user.Type);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserProject", user.Project);

                // نحاول نجيب الـ Agent الحالي بناءً على نوع التعريف
                Agent existingAgent = user.Type switch
                {
                    "NTName" => await _agentRepo.GetByNTNameAsync(user.Identifier),
                    "HRID" => await _agentRepo.GetByHRIDAsync(user.Identifier),
                    "LoginID" => await _agentRepo.GetByLoginIDAsync(user.Identifier),
                    _ => null
                };

                // إذا مش موجود نضيفه (لو دوره Agent فقط)
                if (existingAgent == null && user.Role == "Agent")
                {
                    // نحاول نجيب المشروع
                    var allProjects = await _projectRepo.GetAllAsync();
                    var project = allProjects.FirstOrDefault(p => p.Name == user.Project);

                    // لو مش موجود نضيفه
                    if (project == null)
                    {
                        project = new Project { Name = user.Project };
                        await _projectRepo.AddAsync(project);
                    }

                    var newAgent = new Agent
                    {
                        Name = user.Name,
                        NTName = user.Type == "NTName" ? user.Identifier : null,
                        HRID = user.Type == "HRID" ? user.Identifier : null,
                        LoginID = user.Type == "LoginID" ? user.Identifier : null,
                        Role = AgentRole.Agent,
                        ProjectId = project.Id
                    };

                    await _agentRepo.AddAsync(newAgent);
                }

                // إعادة التوجيه حسب الدور
                return user.Role switch
                {
                    "Agent" => RedirectToAction("Index", "Agents"),
                    "RTM" => RedirectToAction("Pending", "ShiftSwapRequest"),
                    "TeamLeader" => RedirectToAction("Pending", "ShiftSwapRequest"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError("", "Invalid credentials");
            return View(loginDto);
        }
    }
}


