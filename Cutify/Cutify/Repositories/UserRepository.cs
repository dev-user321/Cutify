using Cutify.Data;
using Cutify.Models;
using Cutify.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Repositories
{
    public class UserRepository : Repository<AppUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<List<AppUser>> GetUsersForPaginationAndSearchAsync(int pageNumber, int pageSize, string search)
        {
            var query = GetQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => EF.Functions.Like(u.Name, $"%{search}%") || EF.Functions.Like(u.LastName, $"%{search}%"));
            }

            int totalUsers = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = await query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            httpContextAccessor.HttpContext.Items["TotalPages"] = totalPages;
            httpContextAccessor.HttpContext.Items["CurrentPage"] = pageNumber;
            httpContextAccessor.HttpContext.Items["SearchQuery"] = search;

            return users;
        }
    }
}
