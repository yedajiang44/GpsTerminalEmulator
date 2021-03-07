using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Interface
{
    public interface IBaseService<T> where T : BaseDto
    {
        Task<string> Add(T dto);

        Task<int> Update(T dto);

        Task<int> Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> dto);

        Task<int> Delete(Expression<Func<T, bool>> whereLambda);

        Task<bool> IsExist(Expression<Func<T, bool>> whereLambda);

        Task<T> Find(Expression<Func<T, bool>> whereLambda);

        Task<List<T>> Query<TFilter>(TFilter filter = null) where TFilter : BaseFilter<T>;

        Task<PageResultDto<T>> QueryWithPage<TFilter>(TFilter filter) where TFilter : BaseFilter<T>;
    }
}
