using Domain.CommonEntity;
using Infrastructure.Repository;
using System.Linq.Expressions;

namespace Application.Services.GeneralServices
{
    public class Service<T> : IService<T> where T : BaseEntity
    {
        private readonly IRepository<T> _repository;

        public Service(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _repository.GetAll();
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await _repository.GetById(id);
        }

        public virtual async Task<T> Add(T entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            T result = await _repository.Add(entity);
            //await _repository.SaveChangesMethod();
            return result;
        }

        public virtual async Task Update(T entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;

            await _repository.Update(entity);
        }

        public virtual async Task Delete(Guid id)
        {
            var entity = await _repository.GetById(id);
            if (entity != null)
            {
                await _repository.Delete(entity);
            }
        }

        public virtual async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await _repository.Find(predicate);
        }

        public virtual async Task<T> FindSingle(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindSingle(predicate);
        }
    }
}