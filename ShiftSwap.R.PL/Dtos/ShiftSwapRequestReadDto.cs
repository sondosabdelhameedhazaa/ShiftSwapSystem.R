using System;

namespace ShiftSwap.R.PL.Dtos
{
    public class ShiftSwapRequestReadDto
    {
        public int Id { get; set; }
        public string RequestorName { get; set; }
        public string TargetName { get; set; }
        public DateTime SwapDate { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
