namespace Cutify.Repositories.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task<int> CountAsync();
        IQueryable<T> GetQueryable();
    }
}
