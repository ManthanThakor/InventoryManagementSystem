using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(Guid id);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> match);
        Task<T?> FindSingle(Expression<Func<T, bool>> match);
        Task<int> SaveChangesMethod();
    }
}
