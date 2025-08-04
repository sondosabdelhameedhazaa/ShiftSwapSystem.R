using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSwap.R.DAL.Models
{
    public class ShiftSchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }                  
        public TimeSpan ShiftStart { get; set; }            
        public TimeSpan ShiftEnd { get; set; }              
        public string Shift { get; set; }                   
        public string LOB { get; set; }                     
        public string Schedule { get; set; }                

        public int AgentId { get; set; }
        public Agent Agent { get; set; }
    }


}
