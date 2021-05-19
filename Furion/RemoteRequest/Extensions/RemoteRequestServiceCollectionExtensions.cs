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
using Furion.RemoteRequest;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 远程请求服务拓展类
    /// </summary>
    [SkipScan]
    public static class RemoteRequestServiceCollectionExtensions
    {
        /// <summary>
        /// 注册远程请求
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRemoteRequest(this IServiceCollection services, Action<IServiceCollection> configure = null)
        {
            // 注册远程请求代理接口
            services.AddScopedDispatchProxyForInterface<HttpDispatchProxy, IHttpDispatchProxy>();

            // 注册默认请求客户端
            services.AddHttpClient();

            // 注册其他客户端
            configure?.Invoke(services);

            return services;
        }
    }
}