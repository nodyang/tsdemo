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

namespace Furion.DatabaseAccessor
{
    /// <summary>
    /// 数据库函数类型
    /// </summary>
    [SkipScan]
    internal enum DbFunctionType
    {
        /// <summary>
        /// 标量函数
        /// </summary>
        Scalar,

        /// <summary>
        /// 表值函数
        /// </summary>
        Table
    }
}