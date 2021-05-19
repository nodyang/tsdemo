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
using System.ComponentModel;

namespace Furion.TaskScheduler
{
    /// <summary>
    /// 任务执行类型
    /// </summary>
    [SkipScan]
    public enum SpareTimeExecuteTypes
    {
        /// <summary>
        /// 并行执行（默认方式）
        /// <para>无需等待上一个完成</para>
        /// </summary>
        [Description("并行执行")]
        Parallel,

        /// <summary>
        /// 串行执行
        /// <para>需等待上一个完成</para>
        /// </summary>
        [Description("串行执行")]
        Serial
    }
}