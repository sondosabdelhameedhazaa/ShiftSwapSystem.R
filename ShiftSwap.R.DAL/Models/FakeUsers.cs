using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSwap.R.DAL.Models
{
    public static class FakeUsers
    {
        public static List<(string Type, string Identifier, string Name, string Role, string Project)> Users = new()
    {
        ("NTName", "AhmedSamir", "Ahmed Samir", "RTM", "ProjectA"),
        ("HRID", "12345", "Sondos Ali", "Agent", "ProjectA"),
        ("LoginID", "9876", "Michael Saad", "TeamLeader", "ProjectB"),
    };
    }

}
