using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSwap.R.BLL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAgentRepository Agents { get; }
        IProjectRepository Projects { get; }
        IShiftScheduleRepository ShiftSchedules { get; }
        IShiftSwapRequestRepository ShiftSwapRequests { get; }
        Task<int> CompleteAsync();
    }
}
