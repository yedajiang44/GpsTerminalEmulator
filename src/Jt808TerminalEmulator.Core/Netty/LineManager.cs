using System.Collections.Concurrent;
using Jt808TerminalEmulator.Model.Dtos;

namespace Jt808TerminalEmulator.Core.Netty;

public class LineManager
{
    private readonly LocationInterpolation locationInterpolation;
    private readonly ConcurrentDictionary<string, LineDto> lines = new();
    public LineManager(LocationInterpolation locationInterpolation)
    {
        this.locationInterpolation = locationInterpolation;
    }
    public void Add(LineDto line) => lines.AddOrUpdate(line.Id, line, (_, __) => line);
    public void Remove(string lineId) => lines.TryRemove(lineId, out _);
    public void Clear() => lines.Clear();

    public void ResetLine(List<LineDto> data)
    {
        lines.Clear();
        data.AsParallel().Select(x => { x.Locations = x.Locations.OrderBy(item => item.Order).ToList(); return x; }).ForAll(x => lines.TryAdd(x.Id, x));
    }

    public LocationDto GetNextLocation(string lineId, LocationDto currentLocation, double speed, int interval, ref int nextIndex, bool reverse = false)
    {
        if (lines.TryGetValue(lineId, out var line))
        {
            if (nextIndex == 0)
            {
                nextIndex++;
                return reverse ? line.Locations.LastOrDefault() : line.Locations.FirstOrDefault();
            }
            return locationInterpolation.GetNextLocation(reverse ? line.Locations.AsQueryable().Reverse().ToList() : line.Locations, currentLocation, speed, interval, ref nextIndex);
        }
        return default;
    }
}