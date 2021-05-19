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

namespace Furion.UnifyResult
{
    /// <summary>
    /// 规范化状态码选项
    /// </summary>
    [SkipScan]
    public sealed class UnifyResultStatusCodesOptions
    {
        /// <summary>
        /// 设置返回 200 状态码列表
        /// <para>默认：401，403，如果设置为 null，则标识所有状态码都返回 200 </para>
        /// </summary>
        public int[] Return200StatusCodes { get; set; } = new[] { 401, 403 };
    }
}