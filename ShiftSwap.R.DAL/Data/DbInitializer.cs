using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.DAL.Models.Enums;
using ShiftSwap.R.DAL.Models;


namespace ShiftSwap.R.DAL.Data
{
    public static class DbInitializer
    {
        public static void Seed(ShiftSwapDbContext context)
        {
            // Seed Projects
            if (!context.Projects.Any())
            {
                context.Projects.AddRange(
                    new Project { Name = "Project Alpha" },
                    new Project { Name = "Project Beta" }
                );
            }

            context.SaveChanges();

            // Seed Team Leaders & Agents
            if (!context.Agents.Any())
            {
                var teamLeader1 = new Agent
                {
                    Name = "Ahmed Ali",
                    HRID = "HR001",
                    LoginID = "Ahmed.A",
                    NTName = "ahmed.ali",
                    Role = AgentRole.TeamLeader,
                    ProjectId = context.Projects.First().Id
                };

                var teamLeader2 = new Agent
                {
                    Name = "Sara Mohamed",
                    HRID = "HR002",
                    LoginID = "Sara.M",
                    NTName = "sara.mohamed",
                    Role = AgentRole.TeamLeader,
                    ProjectId = context.Projects.Skip(1).First().Id
                };

                context.Agents.AddRange(teamLeader1, teamLeader2);
                context.SaveChanges();

                var agent1 = new Agent
                {
                    Name = "Omar Khaled",
                    HRID = "HR003",
                    LoginID = "Omar.K",
                    NTName = "omar.khaled",
                    Role = AgentRole.Agent,
                    ProjectId = teamLeader1.ProjectId,
                    TeamLeaderId = teamLeader1.Id
                };

                var agent2 = new Agent
                {
                    Name = "Mona Adel",
                    HRID = "HR004",
                    LoginID = "Mona.A",
                    NTName = "mona.adel",
                    Role = AgentRole.Agent,
                    ProjectId = teamLeader2.ProjectId,
                    TeamLeaderId = teamLeader2.Id
                };

                context.Agents.AddRange(agent1, agent2);
                context.SaveChanges();

                //// Seed ShiftSchedules
                //context.ShiftSchedules.AddRange(
                //    new ShiftSchedule
                //    {
                //        AgentId = agent1.Id,
                //        ShiftDate = DateTime.Today,
                //        ShiftType = ShiftType.Morning
                //    },
                //    new ShiftSchedule
                //    {
                //        AgentId = agent1.Id,
                //        ShiftDate = DateTime.Today.AddDays(1),
                //        ShiftType = ShiftType.Evening
                //    },
                //    new ShiftSchedule
                //    {
                //        AgentId = agent2.Id,
                //        ShiftDate = DateTime.Today,
                //        ShiftType = ShiftType.Night
                //    }
                //);


            }

            context.SaveChanges();
        }
    }
}
