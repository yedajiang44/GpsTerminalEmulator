﻿namespace Jt808TerminalEmulator.Model.Dtos;

/// <summary>
/// 线路
/// </summary>
public class LineDto : BaseDto
{
    /// <summary>
    /// 别名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 距离
    /// </summary>
    public decimal Distance { get; set; }
    /// <summary>
    /// 基准点数量
    /// </summary>
    public int LocationCount { get; set; }
    /// <summary>
    /// 基准点
    /// </summary>
    public List<LocationDto> Locations { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; }
}