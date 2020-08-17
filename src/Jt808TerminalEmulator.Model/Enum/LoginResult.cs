using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Enum
{
    /// <summary>
    /// 登陆结果
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// 账号不存在
        /// </summary>
        AccountNotExists = 1,

        /// <summary>
        /// 登陆密码错误
        /// </summary>
        WrongPassword = 2,

        /// <summary>
        /// 登陆成功
        /// </summary>
        Success = 3
    }
}
