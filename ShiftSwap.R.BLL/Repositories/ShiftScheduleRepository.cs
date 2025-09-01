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
                                 .Where(s => s.Date >= startDate.Date
                                          && s.Date < endDate.Date.AddDays(1)
                                          && s.Agent.ProjectId == projectId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ShiftSchedule>> GetSchedulesForAgentAsync(int agentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.ShiftSchedules
                                .Where(s => s.AgentId == agentId);

            if (fromDate.HasValue)
                query = query.Where(s => s.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(s => s.Date < toDate.Value.Date.AddDays(1));

            return await query.ToListAsync();
        }
    }
}

