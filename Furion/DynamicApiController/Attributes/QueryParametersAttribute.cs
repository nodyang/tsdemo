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
using System;

namespace Furion.DynamicApiController
{
    /// <summary>
    /// 将 Action 所有参数 [FromQuery] 化
    /// </summary>
    [SkipScan, AttributeUsage(AttributeTargets.Method)]
    public sealed class QueryParametersAttribute : Attribute
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public QueryParametersAttribute()
        {
        }
    }
}