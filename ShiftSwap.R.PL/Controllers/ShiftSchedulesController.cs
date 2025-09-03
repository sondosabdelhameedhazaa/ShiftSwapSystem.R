using Microsoft.AspNetCore.Mvc;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftSwap.R.PL.Controllers
{
    public class ShiftScheduleController : Controller
    {
        private readonly IShiftScheduleRepository _shiftScheduleRepo;
        private readonly IAgentRepository _agentRepo;

        public ShiftScheduleController(IShiftScheduleRepository shiftScheduleRepo, IAgentRepository agentRepo)
        {
            _shiftScheduleRepo = shiftScheduleRepo;
            _agentRepo = agentRepo;
        }

        // GET: ShiftSchedule/MySchedule
        public async Task<IActionResult> MySchedule(string? week, DateTime? day)
        {
            var loginId = HttpContext.Session.GetString("LoginID");
            if (string.IsNullOrEmpty(loginId))
                return RedirectToAction("Login", "Account");

            var agent = await _agentRepo.FindFirstAsync(a => a.LoginID == loginId);
            if (agent == null)
                return NotFound("Agent not found");

            var today = DateTime.Today;
            var calendar = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            int currentWeek = calendar.GetWeekOfYear(today,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Sunday);
            int totalWeeks = calendar.GetWeekOfYear(new DateTime(today.Year, 12, 31),
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Sunday);

            int selectedWeek = DetermineSelectedWeek(week, currentWeek, totalWeeks);
            DateTime startOfWeek = FirstDateOfWeek(today.Year, selectedWeek);
            DateTime endOfWeek = startOfWeek.AddDays(6); // الآن الأسبوع كامل (الأحد → السبت)

            var schedules = await _shiftScheduleRepo.GetSchedulesForAgentAsync(agent.Id, startOfWeek, endOfWeek);

            if (day.HasValue)
            {
                schedules = schedules.Where(s => s.Date.Date == day.Value.Date).ToList();
                startOfWeek = day.Value.Date;
                endOfWeek = day.Value.Date;
            }

            var result = schedules.Select(s => new ShiftScheduleDto
            {
                Id = s.Id,
                Date = s.Date,
                ShiftStart = s.ShiftStart,
                ShiftEnd = s.ShiftEnd,
                Shift = s.Shift,
                LOB = s.LOB,
                Schedule = s.Schedule,
                AgentId = s.AgentId,
                AgentName = s.Agent?.Name,
                AgentHRID = s.Agent?.HRID
            }).ToList();

            ViewBag.AgentName = agent.Name;
            ViewBag.AgentHRID = agent.HRID;
            ViewBag.StartDate = startOfWeek.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endOfWeek.ToString("yyyy-MM-dd");
            ViewBag.SelectedWeek = selectedWeek;
            ViewBag.CurrentWeek = currentWeek;
            ViewBag.TotalWeeks = totalWeeks;

            return View(result);
        }

        // GET: ShiftSchedule/TeamSchedule
        public async Task<IActionResult> TeamSchedule(string? week, DateTime? day)
        {
            var loginId = HttpContext.Session.GetString("LoginID");
            if (string.IsNullOrEmpty(loginId))
                return RedirectToAction("Login", "Account");

            var teamLeader = await _agentRepo.FindFirstAsync(a => a.LoginID == loginId);
            if (teamLeader == null)
                return NotFound("Team Leader not found");

            var teamAgents = await _agentRepo.FindAsync(a => a.ProjectId == teamLeader.ProjectId);

            var today = DateTime.Today;
            var calendar = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            int currentWeek = calendar.GetWeekOfYear(today,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Sunday);
            int totalWeeks = calendar.GetWeekOfYear(new DateTime(today.Year, 12, 31),
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Sunday);

            int selectedWeek = DetermineSelectedWeek(week, currentWeek, totalWeeks);
            DateTime startOfWeek = FirstDateOfWeek(today.Year, selectedWeek);
            DateTime endOfWeek = startOfWeek.AddDays(6); // الآن الأسبوع كامل (الأحد → السبت)

            var schedules = new List<ShiftSchedule>();
            foreach (var agent in teamAgents)
            {
                var agentSchedules = await _shiftScheduleRepo.GetSchedulesForAgentAsync(agent.Id, startOfWeek, endOfWeek);
                schedules.AddRange(agentSchedules);
            }

            if (day.HasValue)
            {
                schedules = schedules.Where(s => s.Date.Date == day.Value.Date).ToList();
                startOfWeek = day.Value.Date;
                endOfWeek = day.Value.Date;
            }

            var result = schedules.Select(s => new ShiftScheduleDto
            {
                Id = s.Id,
                Date = s.Date,
                ShiftStart = s.ShiftStart,
                ShiftEnd = s.ShiftEnd,
                Shift = s.Shift,
                LOB = s.LOB,
                Schedule = s.Schedule,
                AgentId = s.AgentId,
                AgentName = s.Agent?.Name,
                AgentHRID = s.Agent?.HRID
            }).OrderBy(s => s.Date).ThenBy(s => s.AgentName).ToList();

            ViewBag.TeamLeaderName = teamLeader.Name;
            ViewBag.StartDate = startOfWeek.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endOfWeek.ToString("yyyy-MM-dd");
            ViewBag.SelectedWeek = selectedWeek;
            ViewBag.CurrentWeek = currentWeek;
            ViewBag.TotalWeeks = totalWeeks;

            return View(result);
        }

        private int DetermineSelectedWeek(string? week, int currentWeek, int totalWeeks)
        {
            if (string.IsNullOrEmpty(week) || week == "this")
                return currentWeek;
            else if (week == "next")
                return Math.Min(currentWeek + 1, totalWeeks);
            else if (int.TryParse(week, out int parsedWeek))
                return Math.Min(Math.Max(1, parsedWeek), totalWeeks);
            else
                return currentWeek;
        }

        private DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Sunday - jan1.DayOfWeek;
            DateTime firstSunday = jan1.AddDays(daysOffset);

            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            int firstWeek = cal.GetWeekOfYear(jan1,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Sunday);

            if (firstWeek <= 1)
                weekOfYear -= 1;

            return firstSunday.AddDays(weekOfYear * 7);
        }
    }
}
