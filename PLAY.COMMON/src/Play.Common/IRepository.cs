using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Play.Common
{
    public interface IRepository<T> where T: IEntity{
        Task<ReadOnlyCollection<T>> GetAllAsync();
        Task<ReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter);
        Task<T> GetAsync(Guid id);
        Task<T> GetAsync(Expression<Func<T,bool>> filter);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);

    }
}