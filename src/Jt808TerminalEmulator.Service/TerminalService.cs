﻿using AutoMapper;
using Bogus;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Service
{
    internal class TerminalService : ITerminalService
    {
        readonly EmulatorDbContext dbContext;
        readonly IMapper mapper;

        public TerminalService(EmulatorDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public Task Add(TerminalDto dto)
        {
            var entity = mapper.Map<TerminalEntity>(dto);
            dbContext.Add(entity);
            return dbContext.SaveChangesAsync();
        }

        public async Task<int> AddRandom(int count)
        {
            var entitys = new Faker<TerminalEntity>("zh_CN")
                .StrictMode(true)
                .RuleFor(x => x.Id, x => Guid.NewGuid().ToString("N"))
                .RuleFor(x => x.IsDeleted, x => false)
                .RuleFor(x => x.CreateUserId, x => null)
                .RuleFor(x => x.CreateDateTime, x => x.Date.Past())
                .RuleFor(x => x.LicensePlate, x => x.Random.Replace("??#####"))
                .RuleFor(x => x.Sim, x => x.Phone.PhoneNumber("###########"))
                .Generate(count);
            await dbContext.AddRangeAsync(entitys);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<bool> Delete(string[] ids)
        {
            foreach (var item in ids.Select(x => new TerminalEntity { Id = x }))
            {
                dbContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            };
            return await dbContext.SaveChangesAsync() > 0;
        }

        public Task<TerminalDto> Find(string id)
        {
            var query = dbContext.Set<TerminalEntity>().FirstOrDefault(x => x.Id == id);
            return Task.FromResult(mapper.Map<TerminalDto>(query));
        }

        public Task<IList<TerminalDto>> FindAll()
        {
            var query = dbContext.Set<TerminalEntity>().ToList();
            return Task.FromResult(mapper.Map<IList<TerminalDto>>(query));
        }

        public Task Update(TerminalDto dto)
        {
            var entity = mapper.Map<TerminalEntity>(dto);
            dbContext.Update(entity);
            return dbContext.SaveChangesAsync();
        }
    }
}
