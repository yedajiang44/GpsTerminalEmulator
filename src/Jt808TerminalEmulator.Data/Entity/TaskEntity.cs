namespace Jt808TerminalEmulator.Data.Entity;

    public class TaskEntity : BaseEntity
{
    /// <summary>
    /// 别名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 定位上报间隔，单位：秒
    /// </summary>
    public int Interval { get; set; }

    /// <summary>
    /// 速度，单位：千米/小时
    /// </summary>
    public double Speed { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 服务器Ip
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 服务器端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 终端
    /// </summary>
    public IList<TerminalEntity> Terminals { get; set; }

    /// <summary>
    /// 线路主键
    /// </summary>
    public string LineId { get; set; }

    /// <summary>
    /// 线路
    /// </summary>
    public LineEntity Line { get; set; }
}