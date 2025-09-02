using System;

namespace ShiftSwap.R.PL.Dtos
{
    public class ShiftScheduleDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public string Shift { get; set; }
        public string LOB { get; set; }
        public string Schedule { get; set; }

        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentHRID { get; set; } 

    }
}

