namespace Jt808TerminalEmulator.Service;

/// <summary>
/// 字符串扩展类
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 用于判断是否为空字符
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsBlank(this string s)
    {
        return s == null || (s.Trim().Length == 0);
    }

    /// <summary>
    /// 用于判断是否为空字符
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNotBlank(this string s)
    {
        return !s.IsBlank();
    }
    /// <summary>
    /// 如果为空，则返回默认值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue">默认值</param>
    /// <returns></returns>
    public static string WithDefaultValueIfEmpty(this string value, string defaultValue)
    {
        return value.IsBlank() ? defaultValue : value;
    }
}