﻿using Jt808TerminalEmulator.Data.Entity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Repository.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        ValueTask<EntityEntry<T>> Add(T entity);

        void Update(T entity);

        Task<int> Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> entity);

        Task<int> Delete(Expression<Func<T, bool>> whereLambda);

        Task<bool> IsExist(Expression<Func<T, bool>> whereLambda);

        Task<T> Find(Expression<Func<T, bool>> whereLambda);

        Task<List<T>> Query(List<(bool ifExpression, Expression<Func<T, bool>> whereExpression)> whereLambdas=null, string order = null);

        Task<Tuple<List<T>, int>> QueryWithPage(List<(bool ifExpression, Expression<Func<T, bool>> whereExpression)> whereLambdas, int intPageIndex = 1, int intPageSize = 20, string order = null);
    }
}
