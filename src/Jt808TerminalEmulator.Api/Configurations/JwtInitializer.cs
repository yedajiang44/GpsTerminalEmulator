using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Jt808TerminalEmulator.Api.Configurations;

/// <summary>
/// json web token 初始化
/// </summary>
public static class JwtInitializer
{
    /// <summary>
    /// 初始化jwt
    /// </summary>
    /// <param name="services"></param>
    /// <param name="Configuration"></param>
    public static IServiceCollection AddJsonWebToken(this IServiceCollection services, IConfiguration Configuration)
    {
        services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            var jwtSettings = services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<JwtSettings>>().CurrentValue;
#if Debug
                cfg.RequireHttpsMetadata = false;
#endif
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters()
            {
                //Token颁发者
                ValidIssuer = jwtSettings.Issuer,
                //使用者
                ValidAudience = jwtSettings.Audience,
                //密钥
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                //验证颁发者签名密钥
                ValidateIssuerSigningKey = true,
                //验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                ValidateLifetime = true,
                //允许的服务器时间偏移量
                ClockSkew = TimeSpan.Zero,
                //验证颁发者
                ValidateIssuer = true,
                //验证使用者
                ValidateAudience = true,
            };
        });
        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
        });
        return services;
    }

    /// <summary>
    /// Jwt配置实体
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 加密的key，必须大于16位
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 过期时间，单位位分钟
        /// </summary>
        public int Expires { get; set; }
    }
}