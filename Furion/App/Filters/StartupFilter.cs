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

using Furion.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Furion
{
    /// <summary>
    /// 应用启动时自动注册中间件
    /// </summary>
    /// <remarks>
    /// </remarks>
    [SkipScan]
    public class StartupFilter : IStartupFilter
    {
        /// <summary>
        /// 配置中间件
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                // 设置响应报文头信息，标记框架类型
                app.Use(async (context, next) =>
                {
                    context.Request.EnableBuffering();  // 启动 Request Body 重复读，解决微信问题

                    await next.Invoke();
                });

                // 调用默认中间件
                app.UseApp();

                // 配置所有 Starup Configure
                UseStartups(app);

                // 调用 Furion.Web.Entry 中的 Startup
                next(app);
            };
        }

        /// <summary>
        /// 配置 Startup 的 Configure
        /// </summary>
        /// <param name="app">应用构建器</param>
        private static void UseStartups(IApplicationBuilder app)
        {
            // 反转，处理排序
            var startups = App.AppStartups.Reverse();
            if (!startups.Any()) return;

            // 遍历所有
            foreach (var startup in startups)
            {
                var type = startup.GetType();

                // 获取所有符合依赖注入格式的方法，如返回值void，且第一个参数是 IApplicationBuilder 类型
                var configureMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(u => u.ReturnType == typeof(void)
                        && u.GetParameters().Length > 0
                        && u.GetParameters().First().ParameterType == typeof(IApplicationBuilder));

                if (!configureMethods.Any()) continue;

                // 自动安装属性调用
                foreach (var method in configureMethods)
                {
                    method.Invoke(startup, ResolveMethodParameterInstances(app, method));
                }
            }
        }

        /// <summary>
        /// 解析方法参数实例
        /// </summary>
        /// <param name="app"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static object[] ResolveMethodParameterInstances(IApplicationBuilder app, MethodInfo method)
        {
            // 获取方法所有参数
            var parameters = method.GetParameters();
            var parameterInstances = new object[parameters.Length];
            parameterInstances[0] = app;

            // 解析服务
            for (int i = 1; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                parameterInstances[i] = app.ApplicationServices.GetRequiredService(parameter.ParameterType);
            }
            return parameterInstances;
        }
    }
}