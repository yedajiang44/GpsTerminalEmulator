using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

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
    }
}
