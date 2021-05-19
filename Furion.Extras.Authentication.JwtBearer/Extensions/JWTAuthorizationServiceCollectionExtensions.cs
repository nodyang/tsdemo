﻿// -----------------------------------------------------------------------------
// 让 .NET 开发更简单，更通用，更流行。
// Copyright © 2020-2021 Furion, 百小僧, Baiqian Co.,Ltd.
//
// 框架名称：Furion
// 框架作者：百小僧
// 框架版本：2.6.0
// 源码地址：Gitee： https://gitee.com/dotnetchina/Furion
//          Github：https://github.com/monksoul/Furion
// 开源协议：Apache-2.0（https://gitee.com/dotnetchina/Furion/blob/master/LICENSE）
// -----------------------------------------------------------------------------

using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// JWT 授权服务拓展类
    /// </summary>
    public static class JWTAuthorizationServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 JWT 授权
        /// </summary>
        /// <param name="authenticationBuilder"></param>
        /// <param name="tokenValidationParameters">token 验证参数</param>
        /// <param name="jwtBearerConfigure"></param>
        /// <param name="enableGlobalAuthorize">启动全局授权</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwt(this AuthenticationBuilder authenticationBuilder, object tokenValidationParameters = default, Action<JwtBearerOptions> jwtBearerConfigure = null, bool enableGlobalAuthorize = false)
        {
            var services = authenticationBuilder.Services;

            // 配置 JWT 选项
            ConfigureJWTOptions(services);

            // 获取配置选项
            var jwtSettings = services.BuildServiceProvider().GetService<IOptions<JWTSettingsOptions>>().Value;

            // 添加授权
            authenticationBuilder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = (tokenValidationParameters as TokenValidationParameters) ?? JWTEncryption.CreateTokenValidationParameters(jwtSettings);

                // 添加自定义配置
                jwtBearerConfigure?.Invoke(options);
            });

            //启用全局授权
            if (enableGlobalAuthorize)
            {
                services.Configure<MvcOptions>(options =>
                {
                    options.Filters.Add(new AuthorizeFilter());
                });
            }

            return authenticationBuilder;
        }

        /// <summary>
        /// 添加 JWT 授权
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authenticationConfigure">授权配置</param>
        /// <param name="tokenValidationParameters">token 验证参数</param>
        /// <param name="jwtBearerConfigure"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwt(this IServiceCollection services, Action<AuthenticationOptions> authenticationConfigure = null, object tokenValidationParameters = default, Action<JwtBearerOptions> jwtBearerConfigure = null)
        {
            // 配置 JWT 选项
            ConfigureJWTOptions(services);

            var jwtSettings = services.BuildServiceProvider().GetService<IOptions<JWTSettingsOptions>>().Value;

            // 添加默认授权
            return services.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                 // 添加自定义配置
                 authenticationConfigure?.Invoke(options);
             }).AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = (tokenValidationParameters as TokenValidationParameters) ?? JWTEncryption.CreateTokenValidationParameters(jwtSettings);

                 // 添加自定义配置
                 jwtBearerConfigure?.Invoke(options);
             });
        }

        /// <summary>
        /// 添加 JWT 授权
        /// </summary>
        /// <typeparam name="TAuthorizationHandler"></typeparam>
        /// <param name="services"></param>
        /// <param name="authenticationConfigure"></param>
        /// <param name="tokenValidationParameters"></param>
        /// <param name="jwtBearerConfigure"></param>
        /// <param name="enableGlobalAuthorize"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwt<TAuthorizationHandler>(this IServiceCollection services, Action<AuthenticationOptions> authenticationConfigure = null, object tokenValidationParameters = default, Action<JwtBearerOptions> jwtBearerConfigure = null, bool enableGlobalAuthorize = false)
            where TAuthorizationHandler : class, IAuthorizationHandler
        {
            // 加载 Furion 程序集
            var furionAssembly = Assembly.Load("Furion");

            // 获取添加授权类型
            var authorizationServiceCollectionExtensionsType = furionAssembly.GetType("Microsoft.Extensions.DependencyInjection.AuthorizationServiceCollectionExtensions");
            var addAppAuthorizationMethod = authorizationServiceCollectionExtensionsType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(u => u.Name == "AddAppAuthorization" && u.IsGenericMethod && u.GetParameters().Length > 0 && u.GetParameters()[0].ParameterType == typeof(IServiceCollection)).First();

            // 添加策略授权服务
            addAppAuthorizationMethod.MakeGenericMethod(typeof(TAuthorizationHandler)).Invoke(null, new object[] { services, null, enableGlobalAuthorize });

            // 添加授权
            return services.AddJwt(authenticationConfigure, tokenValidationParameters, jwtBearerConfigure);
        }

        /// <summary>
        /// 添加 JWT 授权
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureJWTOptions(IServiceCollection services)
        {
            // 获取配置节点
            var jwtSettingsConfiguration = services.BuildServiceProvider()
                        .GetService<IConfiguration>()
                        .GetSection("JWTSettings");

            // 配置验证
            services.AddOptions<JWTSettingsOptions>()
                        .Bind(jwtSettingsConfiguration)
                        .ValidateDataAnnotations();

            // 选项后期配置
            services.PostConfigure<JWTSettingsOptions>(options =>
            {
                _ = JWTEncryption.SetDefaultJwtSettings(options);
            });
        }
    }
}