using System;
using System.Collections.Generic;

namespace ShiftSwap.R.DAL.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }             

        public ICollection<Agent> Agents { get; set; }  
    }
}
