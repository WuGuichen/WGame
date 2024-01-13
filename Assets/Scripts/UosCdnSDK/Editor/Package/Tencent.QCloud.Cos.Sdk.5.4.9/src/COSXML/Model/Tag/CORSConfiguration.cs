﻿﻿using System;
using System.Collections.Generic;

using System.Text;

namespace COSXML.Model.Tag
{
    /// <summary>
    /// <see cref="https://cloud.tencent.com/document/product/436/8274"/>
    /// </summary>
    public sealed class CORSConfiguration
    {
        /// <summary>
        /// 跨域资源共享配置的信息，最多可以包含100条 CORSRule
        /// <see cref="CORSRule"/>
        /// </summary>
        public List<CORSRule> corsRules;

        public string GetInfo()
        {
            StringBuilder stringBuilder = new StringBuilder("{CORSConfiguration:\n");
            if(corsRules != null){
                foreach(CORSRule corsRule in corsRules){
                    if(corsRule != null) stringBuilder.Append(corsRule.GetInfo()).Append("\n");
                }
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }


        public sealed class CORSRule
        {
            /// <summary>
            /// 配置规则的 ID，可选填
            /// </summary>
            public string id;
            /// <summary>
            /// 允许的访问来源，支持通配符 *, 格式为：协议://域名[:端口]如：http://www.qq.com
            /// </summary>
            public string allowedOrigin;
            /// <summary>
            /// 允许的 HTTP 操作，枚举值：GET，PUT，HEAD，POST，DELETE
            /// </summary>
            public List<string> allowedMethods;
            /// <summary>
            /// 在发送 OPTIONS 请求时告知服务端，接下来的请求可以使用哪些自定义的 HTTP 请求头部，支持通配符 *
            /// </summary>
            public List<string> allowedHeaders;
            /// <summary>
            /// 设置浏览器可以接收到的来自服务器端的自定义头部信息
            /// </summary>
            public List<string> exposeHeaders;
            /// <summary>
            /// 设置 OPTIONS 请求得到结果的有效期
            /// </summary>
            public int maxAgeSeconds;

            public string GetInfo()
            {
                StringBuilder stringBuilder = new StringBuilder("{CORSRule:\n");
                stringBuilder.Append("ID:").Append(id).Append("\n");
                stringBuilder.Append("AllowedOrigin:").Append(allowedOrigin).Append("\n");
                if(allowedMethods != null){
                    foreach (string method in allowedMethods){
                        if(method != null)stringBuilder.Append("AllowedMethod:").Append(method).Append("\n");
                    }
                }
                if(allowedHeaders != null){
                    foreach (string header in allowedHeaders){
                        if(header != null)stringBuilder.Append("AllowedHeader:").Append(header).Append("\n");
                    }
                }
                if(exposeHeaders != null){
                    foreach (string exposeHeader in exposeHeaders){
                        if (exposeHeader != null) stringBuilder.Append("ExposeHeader:").Append(exposeHeader).Append("\n");
                    }
                }
                stringBuilder.Append("MaxAgeSeconds:").Append(maxAgeSeconds).Append("\n");
                stringBuilder.Append("}");
                return stringBuilder.ToString();
            }
        }
    }
}
