using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftSwap.R.DAL.Models.Enums;

namespace ShiftSwap.R.DAL.Models
{
    public class ShiftSwapRequest
    {
        public int Id { get; set; }

        public int RequestorAgentId { get; set; }
        public Agent RequestorAgent { get; set; }

        public int TargetAgentId { get; set; }
        public Agent TargetAgent { get; set; }

        public DateTime SwapDate { get; set; }        
        public SwapStatus Status { get; set; }        
        public string Comment { get; set; }           
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? ApprovedById { get; set; }
        public Agent ApprovedBy { get; set; }
    }

}
