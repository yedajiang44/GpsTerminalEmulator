using AutoMapper;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Jt808TerminalEmulator.Service
{
    public class LineService : ILineService
    {
        readonly EmulatorDbContext dbContext;
        readonly IMapper mapper;

        public LineService(EmulatorDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public Task<int> Add(LineDto dto)
        {
            var entity = mapper.Map<LineEntity>(dto);
            dbContext.Set<LineEntity>().Add(entity);
            return dbContext.SaveChangesAsync();
        }

        public Task<int> Delete(string[] ids)
        {
            return dbContext.Set<LineEntity>().Include(x => x.Locations).Where(x => ids.Contains(x.Id)).DeleteAsync();
        }

        public Task<int> Update(LineDto dto)
        {
            var entity = mapper.Map<LineEntity>(dto);
            dbContext.Set<LineEntity>().Update(entity);
            return dbContext.SaveChangesAsync();
        }

        public Task<LineDto> Fine(string id)
        {
            var entity = dbContext.Set<LineEntity>()
                .Include(x => x.Locations)
                .FirstOrDefault(x => x.Id == id);
            return Task.FromResult(mapper.Map<LineDto>(entity));
        }

        public Task<PageResultDto<LineDto>> Search(LineFilter filter)
        {
            return dbContext.Set<LineEntity>()
                .OrderByDescending(x => x.CreateDateTime)
                .WhereIf(filter.Interval > 0, x => x.Interval == filter.Interval)
                .WhereIf(filter.Speed > 0, x => x.Speed == filter.Speed)
                .PagingAsync(filter.Index, filter.Size)
                .Mapper(x => mapper.Map<PageResultDto<LineDto>>(x));
        }
    }
}
