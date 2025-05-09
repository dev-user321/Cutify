using Cutify.Data;
using Cutify.Models;
using Cutify.Repositories.Repository;

namespace Cutify.Repositories
{
    public class ErrorLogRepository : Repository<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(AppDbContext context) : base(context) { }

        public async Task LogErrorAsync(Exception ex, string source)
        {
            var errorLog = new ErrorLog
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = source,
                Timestamp = DateTime.UtcNow
            };
            await AddAsync(errorLog);
        }
    }
}
