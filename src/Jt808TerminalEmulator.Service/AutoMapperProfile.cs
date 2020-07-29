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
            CreateMap<TerminalDto, TerminalEntity>().ReverseMap();
        }
    }
}
