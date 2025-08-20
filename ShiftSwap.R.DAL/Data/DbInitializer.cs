using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.DAL.Models.Enums;
using ShiftSwap.R.DAL.Models;
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

            var tl1 = context.Agents.FirstOrDefault(a => a.HRID == "TL-HRID-1");
            var tl2 = context.Agents.FirstOrDefault(a => a.HRID == "TL-HRID-2");

            if (tl1 == null)
            {
                tl1 = new Agent
                {
                    Name = "TL1",
                    HRID = "TL-HRID-1",
                    LoginID = "TL1",
                    NTName = "tl1",
                    Role = AgentRole.TeamLeader,
                    ProjectId = project.Id
                };
                context.Agents.Add(tl1);
            }

            if (tl2 == null)
            {
                tl2 = new Agent
                {
                    Name = "TL2",
                    HRID = "TL-HRID-2",
                    LoginID = "TL2",
                    NTName = "tl2",
                    Role = AgentRole.TeamLeader,
                    ProjectId = project.Id
                };
                context.Agents.Add(tl2);
            }

            context.SaveChanges();

            var agent1 = context.Agents.FirstOrDefault(a => a.HRID == "119546");
            var agent2 = context.Agents.FirstOrDefault(a => a.HRID == "134057");
            var agent3 = context.Agents.FirstOrDefault(a => a.HRID == "93358");

            if (agent1 == null)
            {
                agent1 = new Agent
                {
                    Name = "Alaa eldin Mahmoud Ghaly, Mariam",
                    HRID = "119546",
                    LoginID = "agent1@rayacx.com",
                    NTName = "agent1",
                    Role = AgentRole.Agent,
                    ProjectId = project.Id,
                    TeamLeaderId = tl1.Id
                };
                context.Agents.Add(agent1);
            }

            if (agent2 == null)
            {
                agent2 = new Agent
                {
                    Name = "Ali, Adham",
                    HRID = "134057",
                    LoginID = "agent2@rayacx.com",
                    NTName = "agent2",
                    Role = AgentRole.Agent,
                    ProjectId = project.Id,
                    TeamLeaderId = tl1.Id
                };
                context.Agents.Add(agent2);
            }

            if (agent3 == null)
            {
                agent3 = new Agent
                {
                    Name = "Hassan Mohamed Khaled, Nourhan",
                    HRID = "93358",
                    LoginID = "agent3@rayacx.com",
                    NTName = "agent3",
                    Role = AgentRole.Agent,
                    ProjectId = project.Id,
                    TeamLeaderId = tl2.Id
                };
                context.Agents.Add(agent3);
            }

            context.SaveChanges();

            bool shiftExists = context.ShiftSchedules.Any(s =>
                s.AgentId == agent1.Id && s.Date == new DateTime(2025, 8, 10)) ||
                context.ShiftSchedules.Any(s =>
                s.AgentId == agent3.Id && s.Date == new DateTime(2025, 8, 10));

            if (!shiftExists)
            {
                context.ShiftSchedules.AddRange(
                    new ShiftSchedule
                    {
                        AgentId = agent1.Id,
                        Date = new DateTime(2025, 8, 10),
                        ShiftStart = new TimeSpan(7, 0, 0),
                        ShiftEnd = new TimeSpan(16, 0, 0),
                        Shift = "Original",
                        LOB = "Gm-Onstar",
                        Schedule = "AutoTest",
                        CreatedBy = "System Admin",
                        CreatedOn = new DateTime(2025, 8, 7),
                        UpdatedBy = "agent1",
                        UpdatedOn = new DateTime(2025, 8, 9)
                    },
                    new ShiftSchedule
                    {
                        AgentId = agent3.Id,
                        Date = new DateTime(2025, 8, 10),
                        ShiftStart = new TimeSpan(8, 0, 0),
                        ShiftEnd = new TimeSpan(17, 0, 0),
                        Shift = "Swapped",
                        LOB = "Gm-Onstar",
                        Schedule = "AutoTest",
                        CreatedBy = "System Admin",
                        CreatedOn = new DateTime(2025, 8, 7),
                        UpdatedBy = "agent3",
                        UpdatedOn = new DateTime(2025, 8, 9)
                    }
                );

                context.SaveChanges();
            }
        }
    }
}

