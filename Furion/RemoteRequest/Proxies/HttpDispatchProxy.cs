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

using Furion.DataValidation;
using Furion.DependencyInjection;
using Furion.Extensions;
using Furion.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Furion.RemoteRequest
{
    /// <summary>
    /// 远程请求实现类（以下代码还需进一步优化性能，启动时把所有扫描缓存起来）
    /// </summary>
    [SkipScan]
    public class HttpDispatchProxy : AspectDispatchProxy, IDispatchProxy
    {
        /// <summary>
        /// 被代理对象
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// 服务提供器
        /// </summary>
        public IServiceProvider Services { get; set; }

        /// <summary>
        /// 拦截同步方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override object Invoke(MethodInfo method, object[] args)
        {
            throw new NotSupportedException("Please use asynchronous operation mode.");
        }

        /// <summary>
        /// 拦截异步无返回方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async override Task InvokeAsync(MethodInfo method, object[] args)
        {
            var httpclientPart = BuildHttpClientPart(method, args);
            _ = await httpclientPart.SendAsync();
        }

        /// <summary>
        /// 拦截异步带返回方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            var httpclientPart = BuildHttpClientPart(method, args);
            var result = httpclientPart.SendAsAsync<T>();
            return result;
        }

        /// <summary>
        /// 构建 HttpClient 请求部件
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private HttpClientExecutePart BuildHttpClientPart(MethodInfo method, object[] args)
        {
            // 判断方法是否是远程代理请求方法
            if (!method.IsDefined(typeof(HttpMethodBaseAttribute), true)) throw new InvalidOperationException($"{method.Name} is not a valid request proxy method.");

            // 解析方法参数及参数值
            var parameters = method.GetParameters().Select((u, i) => new MethodParameterInfo
            {
                Parameter = u,
                Name = u.Name,
                Value = args[i]
            });

            // 获取请求配置
            var httpMethodBase = method.GetCustomAttribute<HttpMethodBaseAttribute>(true);

            // 创建请求配置对象
            var httpClientPart = new HttpClientExecutePart();
            httpClientPart.SetRequestUrl(httpMethodBase.RequestUrl)
                          .SetHttpMethod(httpMethodBase.Method)
                          .SetTemplates(parameters.ToDictionary(u => u.Name, u => u.Value))
                          .SetRequestScoped(Services);

            // 获取方法所在类型
            var declaringType = method.DeclaringType;

            // 设置请求客户端
            SetClient(method, httpClientPart, declaringType);

            // 设置请求报文头
            SetHeaders(method, parameters, httpClientPart, declaringType);

            // 设置 Url 地址参数
            SetQueries(parameters, httpClientPart);

            // 设置 Body 信息
            SetBody(parameters, httpClientPart);

            // 设置验证
            SetValidation(parameters);

            // 设置序列化
            SetJsonSerialization(method, parameters, httpClientPart, declaringType);

            // 配置全局拦截
            CallGlobalInterceptors(httpClientPart, declaringType);

            // 设置请求拦截
            SetInterceptors(parameters, httpClientPart);

            return httpClientPart;
        }

        /// <summary>
        /// 设置客户端信息
        /// </summary>
        /// <param name="method"></param>
        /// <param name="httpClientPart"></param>
        /// <param name="declaringType"></param>
        private static void SetClient(MethodInfo method, HttpClientExecutePart httpClientPart, Type declaringType)
        {
            // 设置 Client 名称，判断方法是否定义，如果没有再查找声明类
            var clientAttribute = method.IsDefined(typeof(ClientAttribute), true)
                ? method.GetCustomAttribute<ClientAttribute>(true)
                : (
                    declaringType.IsDefined(typeof(ClientAttribute), true)
                    ? declaringType.GetCustomAttribute<ClientAttribute>(true)
                    : default
                );
            if (clientAttribute != null) httpClientPart.SetClient(clientAttribute.Name);
        }

        /// <summary>
        /// 设置 Url 地址参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="httpClientPart"></param>
        private static void SetQueries(IEnumerable<MethodParameterInfo> parameters, HttpClientExecutePart httpClientPart)
        {
            // 配置 Url 地址参数
            var queryParameters = parameters.Where(u => u.Parameter.IsDefined(typeof(QueryStringAttribute), true));
            var parameterQueries = new Dictionary<string, object>();
            foreach (var item in queryParameters)
            {
                var queryStringAttribute = item.Parameter.GetCustomAttribute<QueryStringAttribute>();
                if (item.Value != null) parameterQueries.Add(queryStringAttribute.Alias ?? item.Name, item.Value);
            }
            httpClientPart.SetQueries(parameterQueries);
        }

        /// <summary>
        /// 设置 Body 参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="httpClientPart"></param>
        private static void SetBody(IEnumerable<MethodParameterInfo> parameters, HttpClientExecutePart httpClientPart)
        {
            // 配置 Body 参数，只取第一个
            var bodyParameter = parameters.FirstOrDefault(u => u.Parameter.IsDefined(typeof(BodyAttribute), true));
            if (bodyParameter != null)
            {
                var bodyAttribute = bodyParameter.Parameter.GetCustomAttribute<BodyAttribute>(true);
                httpClientPart.SetBody(bodyParameter.Value, bodyAttribute.ContentType, Encoding.GetEncoding(bodyAttribute.Encoding))
                              .SetValidationState(true, true);   // 开启验证
            }

            // 查找所有贴了 [BodyBytes] 特性的参数
            var bodyBytesParameters = parameters.Where(u => u.Parameter.IsDefined(typeof(BodyBytesAttribute), true));
            if (bodyBytesParameters != null)
            {
                var bodyBytes = new List<(string Name, byte[] Bytes, string FileName)>();
                foreach (var item in bodyBytesParameters)
                {
                    var bodyBytesAttribute = item.Parameter.GetCustomAttribute<BodyBytesAttribute>();
                    if (item.Value != null && item.Value.GetType() == typeof(byte[])) bodyBytes.Add((bodyBytesAttribute.Alias ?? item.Name, (byte[])item.Value, bodyBytesAttribute.FileName));
                }

                httpClientPart.SetBodyBytes(bodyBytes);
            }
        }

        /// <summary>
        /// 设置验证
        /// </summary>
        /// <param name="parameters"></param>
        private static void SetValidation(IEnumerable<MethodParameterInfo> parameters)
        {
            // 验证参数，查询所有配置验证特性的参数，排除 Body 验证
            var validateParameters = parameters.Where(u => u.Parameter.IsDefined(typeof(ValidationAttribute), true) && !u.Parameter.IsDefined(typeof(BodyAttribute), true));
            foreach (var item in validateParameters)
            {
                // 处理空值
                var isRequired = item.Parameter.IsDefined(typeof(RequiredAttribute), true);
                if (isRequired && item.Value == null) throw new InvalidOperationException($"{item.Name} can not be null.");

                // 判断是否是基元类型
                if (item.Parameter.ParameterType.IsRichPrimitive())
                {
                    var validationAttributes = item.Parameter.GetCustomAttributes<ValidationAttribute>(true);
                    item.Value?.Validate(validationAttributes.ToArray());
                }
                else item.Value?.Validate();
            }
        }

        /// <summary>
        /// 设置序列化
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="httpClientPart"></param>
        /// <param name="declaringType"></param>
        private static void SetJsonSerialization(MethodInfo method, IEnumerable<MethodParameterInfo> parameters, HttpClientExecutePart httpClientPart, Type declaringType)
        {
            // 配置序列化
            var jsonSerializationAttribute = method.IsDefined(typeof(JsonSerializationAttribute), true)
                ? method.GetCustomAttribute<JsonSerializationAttribute>(true)
                : (
                    declaringType.IsDefined(typeof(JsonSerializationAttribute), true)
                    ? declaringType.GetCustomAttribute<JsonSerializationAttribute>(true)
                    : default
                );
            if (jsonSerializationAttribute != null)
            {
                var jsonSerializerOptionsParameter = parameters.FirstOrDefault(u => u.Parameter.IsDefined(typeof(JsonSerializerOptionsAttribute), true));
                httpClientPart.SetJsonSerialization(jsonSerializationAttribute.ProviderType, jsonSerializerOptionsParameter?.Value);
            }
        }

        /// <summary>
        /// 调用全局拦截
        /// </summary>
        /// <param name="httpClientPart"></param>
        /// <param name="declareType"></param>
        private static void CallGlobalInterceptors(HttpClientExecutePart httpClientPart, Type declareType)
        {
            // 获取所有静态方法且贴有 [Interceptor] 特性
            var interceptorMethods = declareType.GetMethods()
                                                                  .Where(u => u.IsDefined(typeof(InterceptorAttribute), true));

            foreach (var method in interceptorMethods)
            {
                // 获取拦截器类型
                var interceptor = method.GetCustomAttributes<InterceptorAttribute>().First();
                switch (interceptor.Type)
                {
                    // 加载请求拦截
                    case InterceptorTypes.Request:
                        var onRequesting = (Action<HttpRequestMessage>)Delegate.CreateDelegate(typeof(Action<HttpRequestMessage>), method);
                        httpClientPart.OnRequesting(onRequesting);
                        break;
                    // 加载响应拦截
                    case InterceptorTypes.Response:
                        var onResponsing = (Action<HttpResponseMessage>)Delegate.CreateDelegate(typeof(Action<HttpResponseMessage>), method);
                        httpClientPart.OnResponsing(onResponsing);
                        break;
                    // 加载 Client 配置拦截
                    case InterceptorTypes.Client:
                        var onClientCreating = (Action<HttpClient>)Delegate.CreateDelegate(typeof(Action<HttpClient>), method);
                        httpClientPart.OnClientCreating(onClientCreating);
                        break;
                    // 加载异常拦截
                    case InterceptorTypes.Exception:
                        var onException = (Action<HttpResponseMessage, string>)Delegate.CreateDelegate(typeof(Action<HttpResponseMessage, string>), method);
                        httpClientPart.OnException(onException);
                        break;

                    default: break;
                }
            }
        }

        /// <summary>
        /// 设置请求拦截
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="httpClientPart"></param>
        private static void SetInterceptors(IEnumerable<MethodParameterInfo> parameters, HttpClientExecutePart httpClientPart)
        {
            // 添加方法拦截器
            var Interceptors = parameters.Where(u => u.Parameter.IsDefined(typeof(InterceptorAttribute), true));
            foreach (var item in Interceptors)
            {
                // 获取拦截器类型
                var interceptor = item.Parameter.GetCustomAttribute<InterceptorAttribute>();
                switch (interceptor.Type)
                {
                    // 加载请求拦截
                    case InterceptorTypes.Request:
                        if (item.Value is Action<HttpRequestMessage> onRequesting)
                        {
                            httpClientPart.OnRequesting(onRequesting);
                        }
                        break;
                    // 加载响应拦截
                    case InterceptorTypes.Response:
                        if (item.Value is Action<HttpResponseMessage> onResponsing)
                        {
                            httpClientPart.OnResponsing(onResponsing);
                        }
                        break;
                    // 加载 Client 配置拦截
                    case InterceptorTypes.Client:
                        if (item.Value is Action<HttpClient> onClientCreating)
                        {
                            httpClientPart.OnClientCreating(onClientCreating);
                        }
                        break;
                    // 加载异常拦截
                    case InterceptorTypes.Exception:
                        if (item.Value is Action<HttpResponseMessage, string> onException)
                        {
                            httpClientPart.OnException(onException);
                        }
                        break;

                    default: break;
                }
            }
        }

        /// <summary>
        /// 设置请求报文头
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="httpClientPart"></param>
        /// <param name="declaringType"></param>
        private static void SetHeaders(MethodInfo method, IEnumerable<MethodParameterInfo> parameters, HttpClientExecutePart httpClientPart, Type declaringType)
        {
            // 获取声明类请求报文头
            var declaringTypeHeaders = (declaringType.IsDefined(typeof(HeadersAttribute), true)
                ? declaringType.GetCustomAttributes<HeadersAttribute>(true)
                : Array.Empty<HeadersAttribute>()).ToDictionary(u => u.Key, u => u.Value);

            // 获取方法请求报文头
            var methodHeaders = (method.IsDefined(typeof(HeadersAttribute), true)
                ? method.GetCustomAttributes<HeadersAttribute>(true)
                : Array.Empty<HeadersAttribute>()).ToDictionary(u => u.Key, u => u.Value);

            // 获取参数请求报文头
            var headerParameters = parameters.Where(u => u.Parameter.IsDefined(typeof(HeadersAttribute), true));
            var parameterHeaders = new Dictionary<string, object>();
            foreach (var item in headerParameters)
            {
                var headersAttribute = item.Parameter.GetCustomAttribute<HeadersAttribute>(true);
                if (item.Value != null) parameterHeaders.Add(headersAttribute.Key ?? item.Name, item.Value);
            }

            // 合并所有请求报文头
            var headers = declaringTypeHeaders.AddOrUpdate(methodHeaders).AddOrUpdate(parameterHeaders);
            httpClientPart.SetHeaders(headers);
        }
    }
}