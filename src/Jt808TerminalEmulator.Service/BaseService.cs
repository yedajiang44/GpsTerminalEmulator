using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using Jt808TerminalEmulator.Repository.Base;
using Jt808TerminalEmulator.Repository.UnitOfWork;

namespace Jt808TerminalEmulator.Service;

public class BaseService<TDto, TEntity> : IBaseService<TDto> where TDto : BaseDto where TEntity : BaseEntity
{
    protected IMapper mapper;
    protected IUnitOfWork unitOfWork;
    protected IBaseRepository<TEntity> currentRepository;

    public BaseService(IMapper mapper, IUnitOfWork unitOfWork, IBaseRepository<TEntity> currentRepository)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
        this.currentRepository = currentRepository;
    }

    public virtual async Task<string> Add(TDto dto)
    {
        var entity = mapper.Map<TEntity>(dto);
        await currentRepository.Add(entity);
        await unitOfWork.SaveChangesAsync();
        return entity.Id;
    }

    public virtual Task<int> Delete(Expression<Func<TDto, bool>> whereLambda)
    {
        return currentRepository.Delete(mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereLambda));
    }

    public virtual Task<int> Update(TDto dto)
    {
        currentRepository.Update(mapper.Map<TEntity>(dto));
        return unitOfWork.SaveChangesAsync();
    }

    public virtual Task<int> Update(Expression<Func<TDto, bool>> whereLambda, Expression<Func<TDto, TDto>> dto)
    {
        return currentRepository.Update(mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereLambda), mapper.MapExpression<Expression<Func<TEntity, TEntity>>>(dto));
    }

    public async virtual Task<TDto> Find(Expression<Func<TDto, bool>> whereLambda)
    {
        var entity = await currentRepository.Find(mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereLambda));
        return mapper.Map<TDto>(entity);
    }

    public virtual Task<bool> IsExist(Expression<Func<TDto, bool>> whereLambda)
    {
        return currentRepository.IsExist(mapper.MapExpression<Expression<Func<TEntity, bool>>>(whereLambda));
    }

    public async virtual Task<List<TDto>> Query<TFilter>(TFilter filter) where TFilter : BaseFilter<TDto>
    {
        // TODO 排序
        var entitys = await currentRepository.Query(filter?.WhereLambda().ConvertAll(x => (x.ifExpression, mapper.MapExpression<Expression<Func<TEntity, bool>>>(x.whereExpression))));
        return mapper.Map<List<TDto>>(entitys);
    }

    public async virtual Task<PageResultDto<TDto>> QueryWithPage<TFilter>(TFilter filter) where TFilter : BaseFilter<TDto>
    {
        // TODO 排序
        var (entitys, total) = await currentRepository.QueryWithPage(filter?.WhereLambda().ConvertAll(x => (x.ifExpression, mapper.MapExpression<Expression<Func<TEntity, bool>>>(x.whereExpression))), filter?.Index ?? 1, filter?.Size ?? 20);
        return new PageResultDto<TDto>
        {
            List = mapper.Map<List<TDto>>(entitys),
            Total = total
        };
    }
}