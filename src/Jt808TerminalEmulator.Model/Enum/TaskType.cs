using System.ComponentModel;

namespace Jt808TerminalEmulator.Model.Enum;

/// <summary>
/// 任务类型
/// </summary>
public enum TaskType
{
    [Description("一次，即到达终点时结束")]
    Once,
    [Description("重置循环，即到达终点时回到起点")]
    LoopRestart,
    [Description("往复循环，即到达终点时掉头")]
    LoopBack
}
