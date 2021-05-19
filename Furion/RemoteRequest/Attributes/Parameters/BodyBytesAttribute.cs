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

namespace Furion.RemoteRequest
{
    /// <summary>
    /// 配置 Body Bytes 参数
    /// </summary>
    [SkipScan, AttributeUsage(AttributeTargets.Parameter)]
    public class BodyBytesAttribute : ParameterBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="alias"></param>
        public BodyBytesAttribute(string alias)
        {
            Alias = alias;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="fileName"></param>
        public BodyBytesAttribute(string alias, string fileName)
        {
            Alias = alias;
            FileName = fileName;
        }

        /// <summary>
        /// 参数别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
    }
}