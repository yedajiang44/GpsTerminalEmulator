using Jt808TerminalEmulator.Model.Enum;

namespace Jt808TerminalEmulator.Model.Dtos;

/// <summary>
/// 登陆返回数据
/// </summary>
public class UserLoginDto
{
    /// <summary>
    /// 是否登陆成功
    /// </summary>
    public bool LoginSuccess { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// token最后过期时间
    /// </summary>
    public DateTimeOffset? TokenExpiresIn { get; set; }

    /// <summary>
    /// token类型
    /// </summary>
    public string TokenType { get; set; }

    /// <summary>
    /// 登陆结果
    /// </summary>
    public LoginResult Result { get; set; }

    /// <summary>
    /// 返回的登陆用户数据
    /// </summary>
    public UserDto User { get; set; }
}