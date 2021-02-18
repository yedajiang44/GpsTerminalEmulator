using System.Linq;
using AutoMapper;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Service
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap(typeof(PageResultDto<>), typeof(PageResultDto<>));
            CreateMap<TerminalDto, TerminalEntity>()
                .AfterMap((dto, entity) =>
                {
                    if (string.IsNullOrEmpty(entity.Id)) entity.Init();
                })
                .ReverseMap();
            CreateMap<LineDto, LineEntity>()
                .AfterMap((dto, entity) =>
                {
                    if (string.IsNullOrEmpty(entity.Id)) entity.Init();
                    entity.LocationCount = entity.Locations.Count;
                })
                .ReverseMap();
            CreateMap<LocationDto, LocationEntity>()
                .AfterMap((dto, entity) =>
                {
                    if (string.IsNullOrEmpty(entity.Id)) entity.Init();
                })
                .ReverseMap();
            CreateMap<TaskDto, TaskEntity>()
                .AfterMap((dto, entity) =>
                {
                    if (string.IsNullOrEmpty(entity.Id)) entity.Init();
                }).ReverseMap()
                .ForMember(x => x.LineName, m => m.MapFrom(x => x.Line.Name))
                .ForMember(x => x.SimNumber, m => m.MapFrom(x => x.Terminals.FirstOrDefault().Sim));
        }
    }
}
