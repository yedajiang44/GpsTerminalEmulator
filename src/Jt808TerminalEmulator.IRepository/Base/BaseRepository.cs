using Jt808TerminalEmulator.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using System.Linq.Dynamic.Core;

namespace Jt808TerminalEmulator.Repository.Base
{
    public class BaseRepository<T> where T : BaseEntity
    {
        protected readonly EmulatorDbContext dbContext;

        public BaseRepository(EmulatorDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual ValueTask<EntityEntry<T>> Add(T entity)
        {
            return dbContext.Set<T>().AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            dbContext.Update(entity);
        }

        public async virtual Task<int> Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> entity)
        {
            return await BaseQuery().Where(whereLambda).UpdateAsync(entity);
        }

        public async virtual Task<int> Delete(Expression<Func<T, bool>> whereLambda)
        {
            return await BaseQuery().Where(whereLambda).DeleteAsync();
        }

        public async virtual Task<bool> IsExist(Expression<Func<T, bool>> whereLambda)
        {
            return await BaseQuery().AnyAsync(whereLambda);
        }

        public async virtual Task<T> Find(Expression<Func<T, bool>> whereLambda)
        {
            return await BaseQuery().AsNoTracking().FirstOrDefaultAsync(whereLambda);
        }

        public async virtual Task<List<T>> Query(List<(bool ifExpression, Expression<Func<T, bool>> whereExpression)> whereLambdas = null, string order = null)
        {
            var query = BaseQuery();
            whereLambdas?.ForEach(x => query = query.WhereIf(x.ifExpression, x.whereExpression));
            return await query.ToListAsync();
        }

        public async virtual Task<Tuple<List<T>, long>> QueryWithPage(List<(bool ifExpression, Expression<Func<T, bool>> whereExpression)> whereLambdas, int pageIndex, int pageSize, string order)
        {
            var query = BaseQuery();
            whereLambdas?.ForEach(x => query = query.WhereIf(x.ifExpression, x.whereExpression));
            var total = await query.LongCountAsync();
            if (order != null)
            {
                query = query.OrderBy(order);
            }
            var entities = await query.Skip(pageSize * (pageIndex - 1))
                                      .Take(pageSize).ToListAsync();
            return new Tuple<List<T>, long>(entities, total);
        }

        public virtual IQueryable<T> BaseQuery() => dbContext.Set<T>().AsSplitQuery();
    }
}
