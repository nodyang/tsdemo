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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Furion.ClayObject.Extensions
{
    /// <summary>
    /// ExpandoObject 对象拓展
    /// </summary>
    [SkipScan]
    public static class ExpandoObjectExtensions
    {
        /// <summary>
        /// 将对象转 ExpandoObject 类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpandoObject(this object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value is not ExpandoObject expando)
            {
                expando = new ExpandoObject();
                var dict = (IDictionary<string, object>)expando;

                var dictionary = value.ToDictionary();
                foreach (var kvp in dictionary)
                {
                    dict.Add(kvp);
                }
            }

            return expando;
        }

        /// <summary>
        /// 移除 ExpandoObject 对象属性
        /// </summary>
        /// <param name="expandoObject"></param>
        /// <param name="propertyName"></param>
        public static void RemoveProperty(this ExpandoObject expandoObject, string propertyName)
        {
            if (expandoObject == null)
                throw new ArgumentNullException(nameof(expandoObject));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            ((IDictionary<string, object>)expandoObject).Remove(propertyName);
        }

        /// <summary>
        /// 判断 ExpandoObject 是否为空
        /// </summary>
        /// <param name="expandoObject"></param>
        /// <returns></returns>
        public static bool Empty(this ExpandoObject expandoObject)
        {
            return !((IDictionary<string, object>)expandoObject).Any();
        }

        /// <summary>
        /// 判断 ExpandoObject 是否拥有某属性
        /// </summary>
        /// <param name="expandoObject"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this ExpandoObject expandoObject, string propertyName)
        {
            if (expandoObject == null)
                throw new ArgumentNullException(nameof(expandoObject));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            return ((IDictionary<string, object>)expandoObject).ContainsKey(propertyName);
        }

        /// <summary>
        /// 实现 ExpandoObject 浅拷贝
        /// </summary>
        /// <param name="expandoObject"></param>
        /// <returns></returns>
        public static ExpandoObject ShallowCopy(this ExpandoObject expandoObject)
        {
            return Copy(expandoObject, false);
        }

        /// <summary>
        /// 实现 ExpandoObject 深度拷贝
        /// </summary>
        /// <param name="expandoObject"></param>
        /// <returns></returns>
        public static ExpandoObject DeepCopy(this ExpandoObject expandoObject)
        {
            return Copy(expandoObject, true);
        }

        /// <summary>
        /// 拷贝 ExpandoObject 对象
        /// </summary>
        /// <param name="original"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        private static ExpandoObject Copy(ExpandoObject original, bool deep)
        {
            var clone = new ExpandoObject();

            var _original = (IDictionary<string, object>)original;
            var _clone = (IDictionary<string, object>)clone;

            foreach (var kvp in _original)
            {
                _clone.Add(
                    kvp.Key,
                    deep && kvp.Value is ExpandoObject eObject ? DeepCopy(eObject) : kvp.Value
                );
            }

            return clone;
        }
    }
}