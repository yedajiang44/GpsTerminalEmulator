using GpsPlatform.Jt808Protocol;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Abstract;

namespace Jt808TerminalEmulator.Core
{
    /// <summary>
    /// 默认数据包转换器
    /// </summary>
    public class DefaultIPackageConverter : IPackageConverter
    {
        // TODO:如果想使用自己的转换器，只需要更换此处即可
        PackageConverter packageConverter;
        public DefaultIPackageConverter(PackageConverter packageConverter)
        {
            this.packageConverter = packageConverter;
        }
        public T Deserialize<T>(byte[] buffer) where T : class => packageConverter.Deserialize(buffer) as T;

        public byte[] Serialize<T>(T package) where T : class => packageConverter.Serialize(package as Jt808PackageInfo);
    }
}