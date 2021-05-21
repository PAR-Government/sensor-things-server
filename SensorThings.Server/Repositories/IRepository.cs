using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(long id);
        Task<long> AddAsync(T item);
        Task UpdateAsync(T item);
        Task Remove(long id);
    }
}
