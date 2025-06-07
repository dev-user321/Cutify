using Cutify.Models;

namespace Cutify.Repositories.Repository
{
    public interface IWorkHourRepository
    {
        Task<List<WorkHour>> GetAllAsync();
    }

}
