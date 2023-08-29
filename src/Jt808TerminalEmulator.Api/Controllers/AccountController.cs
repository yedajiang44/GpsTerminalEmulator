using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static Jt808TerminalEmulator.Api.Configurations.JwtInitializer;

namespace Jt808TerminalEmulator.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    readonly JwtSettings jwtSettings;

    public AccountController(IOptions<JwtSettings> jwtSettingIOptions)
    {
        jwtSettings = jwtSettingIOptions.Value;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public IActionResult Login(LoginDto dto)
    {
        var userLoginDto = new UserLoginDto
        {
            LoginSuccess = dto.UserName == "admin" && dto.Password == "123456"
        };
        if (userLoginDto.LoginSuccess)
        {
            userLoginDto.Result = Model.Enum.LoginResult.Success;
            userLoginDto.User = new UserDto
            {
                Id = "123456789",
                LoginName = dto.UserName,
                Email = "602830483@qq.com",
                RealName = "yedajiang44",
                UserRoles = new List<UserRoleDto>()
            };
            var claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, userLoginDto.User.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, userLoginDto.User.LoginName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    //主键
                    new Claim(ClaimTypes.Sid,userLoginDto.User.Id),
                    //用户名
                    new Claim(ClaimTypes.Name,userLoginDto.User.LoginName),
                };

            //角色
            claims.AddRange(userLoginDto.User.UserRoles.Select(x => new Claim(ClaimTypes.Role, x.RoleName)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)); //密钥
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwtSettings.Issuer,
                jwtSettings.Audience,
                claims,
                //到期时间
                expires: DateTime.Now.AddMinutes(jwtSettings.Expires),
                signingCredentials: creds);

            userLoginDto.Token = new JwtSecurityTokenHandler().WriteToken(token);
            userLoginDto.TokenExpiresIn = DateTime.Now.AddMinutes(jwtSettings.Expires);
            userLoginDto.TokenType = "Bearer";
        }
        else
        {
            return Ok(new JsonResultDto<UserLoginDto> { Flag = false, Message = "账号或密码错误" });
        }
        return Ok(new JsonResultDto<UserLoginDto> { Flag = true, Data = userLoginDto });
    }
}