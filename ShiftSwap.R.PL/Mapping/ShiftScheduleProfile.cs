using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.PL.Mapping
{
    public static class ShiftScheduleProfile
    {
        public static ShiftScheduleDto ToDto(this ShiftSchedule schedule)
        {
            return new ShiftScheduleDto
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
        }
    }

}
