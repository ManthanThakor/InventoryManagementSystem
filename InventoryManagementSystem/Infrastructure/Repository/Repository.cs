using Domain.CommonEntity;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<Repository<T>> _logger;

        public Repository(ApplicationDbContext dbContext, ILogger<Repository<T>> logger)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
            _logger = logger;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            T? entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new Exception($"Entity with ID {id} not found.");
            }
            return entity;
        }

        public async Task<T> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesMethod();
            return entity;
        }

        public async Task Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await SaveChangesMethod();
        }

        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            await SaveChangesMethod();
        }

        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> match)
        {
            return await _dbSet.Where(match).ToListAsync();
        }

        public async Task<T> FindSingle(Expression<Func<T, bool>> match)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(match);
            if (entity == null)
            {
                _logger.LogError("Entity not found for the given criteria.");

                throw new EntityNotFoundException($"{typeof(T).Name} matching the condition was not found.");

            }
            return entity;
        }

        public async Task<int> SaveChangesMethod()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
