using ShiftSwap.R.DAL.Models.Enums;
using System;

namespace ShiftSwap.R.PL.Dtos
{
    public class AgentReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HRID { get; set; }
        public string LoginID { get; set; }
        public string NTName { get; set; }
        public AgentRole Role { get; set; }
        public string? ProjectName { get; set; }
        public string? TeamLeaderName { get; set; }

        public string ShiftStart { get; set; }  
        public string ShiftEnd { get; set; }

        public string? Shift { get; set; }
    }
}
