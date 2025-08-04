using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.BLL.Interfaces
{
    public interface IShiftScheduleRepository : IGenericRepository<ShiftSchedule>
    {
        Task<IEnumerable<ShiftSchedule>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<ShiftSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int projectId);
    }
}
