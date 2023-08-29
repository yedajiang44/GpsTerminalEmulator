namespace Jt808TerminalEmulator.Core.Abstract;

public interface IPackageConverter
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    byte[] Serialize<T>(T package) where T : class;

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    T Deserialize<T>(byte[] buffer) where T : class;
}