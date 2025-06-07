using Cutify.Data;
using Cutify.Models;
using Cutify.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Repositories
{
    public class WorkHourRepository : IWorkHourRepository
    {
        private readonly AppDbContext _context;

        public WorkHourRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkHour>> GetAllAsync()
        {
            return await _context.WorkHours.OrderBy(w => w.Time).ToListAsync();
        }
    }

}
