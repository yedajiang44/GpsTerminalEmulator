namespace Jt808TerminalEmulator.Model.Dtos;

public class BenchmarkListItemDto
{
    /// <summary>
    /// sim卡号
    /// </summary>
    public string SimNumber { get; set; }
    /// <summary>
    /// 车牌
    /// </summary>
    public string LicensePlate { get; set; }
    /// <summary>
    /// 是否在线
    /// </summary>
    public bool Online { get; set; }
    /// <summary>
    /// 上线时间
    /// </summary>
    public DateTime? StartDateTime { get; set; }
    /// <summary>
    /// 最后活动时间
    /// </summary>
    public DateTime? LastActiveDateTime { get; set; }
    /// <summary>
    /// 在线时长
    /// </summary>
    public TimeOnly OnlineTime { get; set; }
}
