using System;
using ShiftSwap.R.BLL.Interfaces;
using ShiftSwap.R.DAL.Data.Contexts;
using ShiftSwap.R.DAL.Models;

namespace ShiftSwap.R.BLL.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ShiftSwapDbContext context) : base(context) { }
    }
}
