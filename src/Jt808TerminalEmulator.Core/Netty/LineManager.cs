using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Jt808TerminalEmulator.Model.Dtos;

namespace Jt808TerminalEmulator.Core.Netty
{
    public class LineManager
    {
        LocationInterpolation locationInterpolation;
        ConcurrentDictionary<string, LineDto> lines = new ConcurrentDictionary<string, LineDto>();
        public LineManager(LocationInterpolation locationInterpolation)
        {
            this.locationInterpolation = locationInterpolation;
        }
        public void Add(LineDto line) => lines.AddOrUpdate(line.Id, line, (key, value) => line);
        public void Remove(string lineId) => lines.TryRemove(lineId, out _);
        public void Clear() => lines.Clear();

        public void ResetLine(List<LineDto> data)
        {
            lines.Clear();
            data.AsParallel().Select(x => { x.Locations = x.Locations.OrderBy(item => item.Order).ToList(); return x; }).ForAll(x => lines.TryAdd(x.Id, x));
        }

        public LocationDto GetNextLocaltion(string lineId, LocationDto currentLocation, double speed, int interval, ref int nextIndex)
        {
            if (lines.TryGetValue(lineId, out var line))
            {
                if (nextIndex == 0)
                {
                    return line.Locations[nextIndex++];
                }
                return locationInterpolation.GetNextLation(line.Locations, currentLocation, speed, interval, ref nextIndex);
            }
            return default;
        }
    }
}