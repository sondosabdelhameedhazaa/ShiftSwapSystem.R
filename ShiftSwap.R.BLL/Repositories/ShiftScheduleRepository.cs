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
    public class ShiftScheduleRepository : GenericRepository<ShiftSchedule>, IShiftScheduleRepository
    {
        public ShiftScheduleRepository(ShiftSwapDbContext context) : base(context) { }

        public async Task<IEnumerable<ShiftSchedule>> GetByAgentIdAsync(int agentId)
        {
            return await _context.ShiftSchedules
                                 .Where(s => s.AgentId == agentId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ShiftSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int projectId)
        {
            return await _context.ShiftSchedules
                                 .Include(s => s.Agent)
                                 .Where(s => s.Date >= startDate
                                          && s.Date <= endDate
                                          && s.Agent.ProjectId == projectId)
                                 .ToListAsync();
        }
    }
}
