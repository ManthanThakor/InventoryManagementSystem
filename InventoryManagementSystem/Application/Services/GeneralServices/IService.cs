using Domain.CommonEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.GeneralServices
{
    public interface IService<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(Guid id);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(Guid id);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> match);
        Task<T> FindSingle(Expression<Func<T, bool>> match);
    }
}