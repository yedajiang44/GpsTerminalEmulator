using GpsPlatform.Jt808Protocol;
using GpsPlatform.Jt808Protocol.PackageInfo;
namespace Jt808TerminalEmulator.Core.Abstract
{
    public interface IPackageConverter
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="packageConfig"></param>
        byte[] Serialize<T>(T package) where T : class;

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="packageConfig"></param>
        /// <returns></returns>
        T Deserialize<T>(byte[] buffer) where T : class;
    }
}