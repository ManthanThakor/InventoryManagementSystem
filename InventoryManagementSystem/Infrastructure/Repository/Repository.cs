using Domain;
using Domain.CommonEntity;
using Infrastructure.Data;
using InfrastructureLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly DbSet<T> entities;

        public Repository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            entities = _applicationDbContext.Set<T>();
        }
        public async Task<T> Get(Guid Id)
        {
            return await entities.FindAsync(Id);
        }

        public async Task<ICollection<T>> GetAll()
        {
            return await entities.ToListAsync();
        }

        public async Task<bool> Insert(T entity)
        {
            await entities.AddAsync(entity);
            var result = await _applicationDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Delete(T entity)
        {
            entities.Remove(entity);
            var result = await _applicationDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public T GetLast()
        {
            return entities.OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public async Task<bool> Update(T entity)
        {
            entities.Update(entity);
            var result = await _applicationDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<T> Find(Expression<Func<T, bool>> match)
        {
            return await entities.FirstOrDefaultAsync(match);
        }

        public async Task<ICollection<T>> FindAll(Expression<Func<T, bool>> match)
        {
            return await entities.Where(match).ToListAsync();
        }
    }
}
