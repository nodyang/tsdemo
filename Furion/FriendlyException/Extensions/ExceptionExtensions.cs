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
using Furion.UnifyResult;
using Microsoft.AspNetCore.Http;
using System;

namespace Furion.FriendlyException
{
    /// <summary>
    /// 异常拓展
    /// </summary>
    [SkipScan]
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 设置异常状态码
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static Exception StatusCode(this Exception exception, int statusCode = StatusCodes.Status500InternalServerError)
        {
            UnifyContext.Set(UnifyContext.UnifyResultStatusCodeKey, statusCode);
            return exception;
        }
    }
}