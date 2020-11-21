using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Service
{
    public static class QueryableExtention
    {
        /// <summary>
        /// 自定义排序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="query">查询对象</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="sord">排序方式</param>
        /// <returns></returns>
        public static IQueryable<T> OrderByCustom<T>(this IQueryable<T> query, string fieldName, string sord)
        {
            var fields = fieldName.WithDefaultValueIfEmpty("CreateDateTime");
            sord = sord.WithDefaultValueIfEmpty("desc");
            var sorts = string.Format(" {0} {1} ", fields, sord);
            return query.OrderBy(sorts);
        }

        /// <summary>
        /// WhereIf[在condition为true的情况下应用Where表达式]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> expression)
        {
            return condition ? source.Where(expression) : source;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static PageResultDto<T> Paging<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
                pageIndex = 1;
            if (pageSize <= 1)
                pageSize = 20;
            var pagedResult = source.PageResult(pageIndex, pageSize);

            return new PageResultDto<T>
            {
                List = pagedResult.Queryable.ToList(),
                Total = pagedResult.RowCount,
            };
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static async Task<PageResultDto<T>> PagingAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
                pageIndex = 1;
            if (pageSize <= 1)
                pageSize = 20;
            var pagedResult = source.PageResult(pageIndex, pageSize);
            return new PageResultDto<T>
            {
                List = await pagedResult.Queryable.ToListAsync(),
                Total = pagedResult.RowCount,
            };
        }

        public static PageResultDto<R> Mapper<T, R>(this PageResultDto<T> source, Func<PageResultDto<T>, PageResultDto<R>> func) => func(source);

        public static async Task<PageResultDto<R>> Mapper<T, R>(this Task<PageResultDto<T>> source, Func<PageResultDto<T>, PageResultDto<R>> func) => await Task.FromResult(func(await source));
    }
}
