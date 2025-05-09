using Cutify.Data;
using Cutify.Models;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Repositories.Repository
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<List<AppUser>> GetUsersForPaginationAndSearchAsync(int pageNumber, int pageSize, string search);
    }

    
}
