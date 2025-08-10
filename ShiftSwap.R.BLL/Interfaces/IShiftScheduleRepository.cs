using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.BLL.Interfaces
{
    public interface IShiftScheduleRepository : IGenericRepository<ShiftSchedule>
    {
        Task<IEnumerable<ShiftSchedule>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<ShiftSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int projectId);

        Task<IEnumerable<ShiftSchedule>> GetSchedulesForAgentAsync(int agentId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
