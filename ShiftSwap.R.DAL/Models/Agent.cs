using System;
using System.Collections.Generic;
using ShiftSwap.R.DAL.Models.Enums;

namespace ShiftSwap.R.DAL.Models
{
    public class Agent
    {
        public int Id { get; set; }

        public string Name { get; set; }              
        public string HRID { get; set; }              
        public string LoginID { get; set; }           
        public string NTName { get; set; }            
        public AgentRole Role { get; set; }           

        // Relationships
        public int ProjectId { get; set; }            // Project FK
        public Project Project { get; set; }

        public int? TeamLeaderId { get; set; }        // Team Leader FK
        public Agent TeamLeader { get; set; }         // Navigation to TL

        public ICollection<ShiftSchedule> ShiftSchedules { get; set; }  // Weekly Shifts
        public ICollection<ShiftSwapRequest> SentSwapRequests { get; set; }      
        public ICollection<ShiftSwapRequest> ReceivedSwapRequests { get; set; }  
    }
}

