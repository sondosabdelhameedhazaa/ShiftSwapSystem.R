using Microsoft.AspNetCore.Mvc;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

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

        // GET: ShiftSchedule/Index?agentId=1
        public async Task<IActionResult> Index(int? agentId, DateTime? fromDate, DateTime? toDate)
        {
            if (agentId == null)
                return BadRequest("AgentId is required");

            var schedules = await _shiftScheduleRepo.GetSchedulesForAgentAsync(agentId.Value, fromDate, toDate);

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
                AgentName = s.Agent?.Name
            }).ToList();

            ViewBag.AgentName = schedules.FirstOrDefault()?.Agent?.Name ?? "Agent";

            return View(result);
        }

        // GET: ShiftSchedule/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var schedule = await _shiftScheduleRepo.GetByIdAsync(id);
            if (schedule == null)
                return NotFound();

            var dto = new ShiftScheduleDto
            {
                Id = schedule.Id,
                Date = schedule.Date,
                ShiftStart = schedule.ShiftStart,
                ShiftEnd = schedule.ShiftEnd,
                Shift = schedule.Shift,
                LOB = schedule.LOB,
                Schedule = schedule.Schedule,
                AgentId = schedule.AgentId,
                AgentName = schedule.Agent?.Name
            };

            return View(dto);
        }

        // GET: ShiftSchedule/DownloadExcel?agentId=1&fromDate=...&toDate=...
        public async Task<IActionResult> DownloadExcel(int agentId, DateTime? fromDate, DateTime? toDate)
        {
            var schedules = await _shiftScheduleRepo.GetSchedulesForAgentAsync(agentId, fromDate, toDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Shift Schedule");

            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "Shift Start";
            worksheet.Cell(1, 3).Value = "Shift End";
            worksheet.Cell(1, 4).Value = "Shift";
            worksheet.Cell(1, 5).Value = "LOB";
            worksheet.Cell(1, 6).Value = "Schedule";

            int row = 2;
            foreach (var s in schedules)
            {
                worksheet.Cell(row, 1).Value = s.Date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 2).Value = s.ShiftStart.ToString();
                worksheet.Cell(row, 3).Value = s.ShiftEnd.ToString();
                worksheet.Cell(row, 4).Value = s.Shift;
                worksheet.Cell(row, 5).Value = s.LOB;
                worksheet.Cell(row, 6).Value = s.Schedule;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ShiftSchedule_Agent{agentId}.xlsx");
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

            // نحسب الأسبوع الحالي وعدد أسابيع السنة
            var calendar = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            int currentWeek = calendar.GetWeekOfYear(today,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Sunday);

            int totalWeeks = calendar.GetWeekOfYear(new DateTime(today.Year, 12, 31),
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Sunday);

            int selectedWeek;

            if (string.IsNullOrEmpty(week) || week == "this")
            {
                selectedWeek = currentWeek; // This week
            }
            else if (week == "next")
            {
                selectedWeek = currentWeek + 1; // Next week
                if (selectedWeek > totalWeeks) selectedWeek = totalWeeks;
            }
            else if (int.TryParse(week, out int parsedWeek)) 
            {
                selectedWeek = Math.Min(Math.Max(1, parsedWeek), totalWeeks);
            }
            else
            {
                selectedWeek = currentWeek;
            }

            DateTime startOfWeek = FirstDateOfWeek(today.Year, selectedWeek);
            DateTime endOfWeek = startOfWeek.AddDays(4); 

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
                AgentName = s.Agent?.Name
            }).ToList();

            ViewBag.AgentName = agent.Name;
            ViewBag.StartDate = startOfWeek.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endOfWeek.ToString("yyyy-MM-dd");
            ViewBag.SelectedWeek = selectedWeek;
            ViewBag.CurrentWeek = currentWeek;
            ViewBag.TotalWeeks = totalWeeks;

            return View(result);
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

