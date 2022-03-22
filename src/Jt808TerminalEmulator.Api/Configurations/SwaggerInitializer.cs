using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Jt808TerminalEmulator.Api.Configurations
{
    public static class SwaggerInitializer
    {

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("公开接口", new OpenApiInfo
                {
                    Title = "公开通用接口Api",
                    Version = "bate 0.0.1",
                    Description = "无需登录的通用接口",
                    Contact = new OpenApiContact
                    {
                        Name = "yedajiang44",
                        Email = "602830483@qq.com",
                        Url = new Uri("http://www.github.com/yedajiang44"),
                    }
                });
                c.SwaggerDoc("后台接口", new OpenApiInfo
                {
                    Title = "系统 API",
                    Version = "bate 0.0.1",
                    Description = "用户、权限、菜单等系统级相关的接口",
                    Contact = new OpenApiContact
                    {
                        Name = "yedajiang44",
                        Email = "602830483@qq.com"
                    }
                });
                // 使用反射获取xml文件。并构造出文件的路径
                // 启用xml注释. 该方法第二个参数启用控制器的注释，默认为false.
                // c.IncludeXmlComments($"Yedajiang44.Api.xml", true);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入登录接口返回的Token，并前置Bearer，示例：Bearer { Roken }",
                    Name = "Authorization",
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        Array.Empty<string>()
                    }
                 });
            });
        }

        public static void UseSwaggerAndUI(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/公开接口/swagger.json", "公开接口");
                c.SwaggerEndpoint("/swagger/后台接口/swagger.json", "后台接口");
#if DEBUG
                c.RoutePrefix = string.Empty;
#endif
            });
        }
    }
}