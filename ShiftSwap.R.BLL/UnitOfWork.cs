using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.BLL.Repositories;
using ShiftSwap.R.DAL.Data.Contexts;

namespace ShiftSwap.R.BLL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ShiftSwapDbContext _context;

        public IAgentRepository Agents { get; private set; }
        public IProjectRepository Projects { get; private set; }
        public IShiftScheduleRepository ShiftSchedules { get; private set; }
        public IShiftSwapRequestRepository ShiftSwapRequests { get; private set; }

        public UnitOfWork(ShiftSwapDbContext context)
        {
            _context = context;
            Agents = new AgentRepository(_context);
            Projects = new ProjectRepository(_context);
            ShiftSchedules = new ShiftScheduleRepository(_context);
            ShiftSwapRequests = new ShiftSwapRequestRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
