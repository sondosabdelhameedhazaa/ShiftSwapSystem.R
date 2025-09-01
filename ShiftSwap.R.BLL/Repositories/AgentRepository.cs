using Microsoft.EntityFrameworkCore;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftSwap.R.BLL.Repositories
{
    public class AgentRepository : GenericRepository<Agent>, IAgentRepository
    {
        public AgentRepository(ShiftSwapDbContext context) : base(context) { }

        public async Task<Agent> GetByHRIDAsync(string hrid) =>
            await _context.Agents.Include(a => a.Project).FirstOrDefaultAsync(a => a.HRID == hrid);

        public async Task<Agent> GetByLoginIDAsync(string loginId) =>
            await _context.Agents.Include(a => a.Project).FirstOrDefaultAsync(a => a.LoginID == loginId);

        public async Task<Agent> GetByNTNameAsync(string ntName) =>
            await _context.Agents.Include(a => a.Project).FirstOrDefaultAsync(a => a.NTName == ntName);

        public async Task<IEnumerable<Agent>> GetAgentsByProjectAsync(int projectId) =>
            await _context.Agents
                .Where(a => a.ProjectId == projectId)
                .Include(a => a.Project)
                .ToListAsync();

        public async Task<IEnumerable<(Agent Agent, ShiftSchedule Schedule)>> GetAvailableAgentsWithShiftsAsync(DateTime date, int excludeAgentId)
        {
            var result = await _context.ShiftSchedules
                .Include(s => s.Agent)
                    .ThenInclude(a => a.Project)
                .Include(s => s.Agent)
                    .ThenInclude(a => a.TeamLeader)
                .Where(s => s.Date.Date == date.Date
                            && s.AgentId != excludeAgentId) 
                .Select(s => new { s.Agent, Schedule = s })
                .ToListAsync();

            return result.Select(r => (r.Agent, r.Schedule));
        }
    }
}



