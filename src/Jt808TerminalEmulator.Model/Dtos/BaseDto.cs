using System;

namespace Jt808TerminalEmulator.Model.Dtos
{
    public class BaseDto : BaseDto<string>
    {
    }
    public class BaseDto<TPrimaryKey> : IBaseDto<TPrimaryKey>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public TPrimaryKey Id { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }
    }
}
