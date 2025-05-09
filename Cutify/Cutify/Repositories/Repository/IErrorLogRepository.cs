using Cutify.Data;
using Cutify.Models;

namespace Cutify.Repositories.Repository
{
    public interface IErrorLogRepository
    {
        Task LogErrorAsync(Exception ex, string source);
    }

    
}
