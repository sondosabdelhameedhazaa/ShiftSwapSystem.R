using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.DAL.Models.Enums;
using System;
using System.Linq;

namespace ShiftSwap.R.DAL.Data
{
    public static class DbInitializer
    {
        public static void Seed(ShiftSwapDbContext context)
        {
            
            var project = context.Projects.FirstOrDefault(p => p.Name == "Gm-Onstar");
            if (project == null)
            {
                project = new Project { Name = "Gm-Onstar" };
                context.Projects.Add(project);
                context.SaveChanges();
            }

            var agent1 = context.Agents.FirstOrDefault(a => a.HRID == "119546") ?? new Agent
            {
                Name = "Alaa eldin Mahmoud Ghaly, Mariam",
                HRID = "119546",
                LoginID = "agent1@rayacx.com",
                NTName = "agent1",
                Role = AgentRole.Agent,
                ProjectId = project.Id
            };

            var agent2 = context.Agents.FirstOrDefault(a => a.HRID == "134057") ?? new Agent
            {
                Name = "Ali, Adham",
                HRID = "134057",
                LoginID = "agent2@rayacx.com",
                NTName = "agent2",
                Role = AgentRole.Agent,
                ProjectId = project.Id
            };

            var agent3 = context.Agents.FirstOrDefault(a => a.HRID == "93358") ?? new Agent
            {
                Name = "Hassan Mohamed Khaled, Nourhan",
                HRID = "93358",
                LoginID = "agent3@rayacx.com",
                NTName = "agent3",
                Role = AgentRole.Agent,
                ProjectId = project.Id
            };

            if (agent1.Id == 0) context.Agents.Add(agent1);
            if (agent2.Id == 0) context.Agents.Add(agent2);
            if (agent3.Id == 0) context.Agents.Add(agent3);

            context.SaveChanges();

            var agents = new[]
            {
                new { Agent = agent1, ShiftStart = new TimeSpan(7,0,0), ShiftEnd = new TimeSpan(16,0,0), ShiftType = "Original" },
                new { Agent = agent2, ShiftStart = new TimeSpan(8,0,0), ShiftEnd = new TimeSpan(17,0,0), ShiftType = "Original" },
                new { Agent = agent3, ShiftStart = new TimeSpan(9,0,0), ShiftEnd = new TimeSpan(18,0,0), ShiftType = "Swapped" }
            };

            // 10 weeks + 1
                 var today = DateTime.Today;
            var currentSunday = today.AddDays(-(int)today.DayOfWeek);
            var weeks = Enumerable.Range(-10, 12) 
                .Select(offset => currentSunday.AddDays(offset * 7))
                .ToList();

            foreach (var weekStart in weeks)
            {
                for (int dayOffset = 0; dayOffset < 5; dayOffset++)
                {
                    var date = weekStart.AddDays(dayOffset);

                    foreach (var a in agents)
                    {
                        if (!context.ShiftSchedules.Any(s => s.Date.Date == date.Date && s.AgentId == a.Agent.Id))
                        {
                            context.ShiftSchedules.Add(new ShiftSchedule
                            {
                                AgentId = a.Agent.Id,
                                Date = date,
                                ShiftStart = a.ShiftStart,
                                ShiftEnd = a.ShiftEnd,
                                Shift = a.ShiftType,
                                LOB = "Gm-Onstar",
                                Schedule = "AutoTest",
                                CreatedBy = "System Admin",
                                CreatedOn = DateTime.Now,
                                UpdatedBy = a.Agent.NTName,
                                UpdatedOn = DateTime.Now
                            });
                        }
                    }
                }
            }

            // make them at same project to test 
            var agentsToAddOrUpdate = new[]
            {
    new { HRID = "119546", Name = "Alaa eldin Mahmoud Ghaly, Mariam", LoginID = "agent1@rayacx.com", NTName = "agent1" },
    new { HRID = "134057", Name = "Ali, Adham", LoginID = "agent2@rayacx.com", NTName = "agent2" },
    new { HRID = "93358", Name = "Hassan Mohamed Khaled, Nourhan", LoginID = "agent3@rayacx.com", NTName = "agent3" }
};

            foreach (var a in agentsToAddOrUpdate)
            {
                var agent = context.Agents.FirstOrDefault(x => x.HRID == a.HRID);
                if (agent == null)
                {
                    agent = new Agent
                    {
                        Name = a.Name,
                        HRID = a.HRID,
                        LoginID = a.LoginID,
                        NTName = a.NTName,
                        Role = AgentRole.Agent,
                        ProjectId = project.Id
                    };
                    context.Agents.Add(agent);
                }
                else
                {
                    if (agent.ProjectId != project.Id)
                    {
                        agent.ProjectId = project.Id;
                        context.Agents.Update(agent);
                    }
                }
            }

            context.SaveChanges();


            context.SaveChanges();
        }
    }
}


