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
}
