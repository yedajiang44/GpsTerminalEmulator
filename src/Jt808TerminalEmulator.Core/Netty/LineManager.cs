using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace Jt808TerminalEmulator.Core.Netty
{
    public class LineManager
    {
        static ConcurrentDictionary<string, LineDto> lines = new ConcurrentDictionary<string, LineDto>();
        public static void Add(LineDto line) => lines.TryAdd(line.Id, line);
        public static void Remove(string lineId) => lines.TryRemove(lineId, out _);
        public static void Clear() => lines.Clear();

        public static void ResetLine(List<LineDto> data)
        {
            lines.Clear();
            data.AsParallel().Select(x => {x.Locations= x.Locations.OrderBy(item => item.Order).ToList(); return x; }).ForAll(x => lines.TryAdd(x.Id, x));
        }

        public static LocationDto GetNextLocaltion(string lineId, LocationDto currentLocation, double speed, int interval, ref int nextIndex)
        {
            if (lines.TryGetValue(lineId, out var line))
            {
                if (nextIndex == 0)
                {
                    return line.Locations[nextIndex++];
                }
                return LocationInterpolation.GetNextLation(line.Locations, currentLocation, speed, interval, ref nextIndex);
            }
            return default;
        }
    }
}