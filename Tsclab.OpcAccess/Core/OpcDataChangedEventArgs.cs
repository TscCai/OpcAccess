using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC Core
    /// 摘要：定义OPCDataChangedEventArgs
    /// 作者：Tsccai
    /// 编写日期：2013/3/25 22:28:05
    /// </summary>
    public class OpcDataChangedEventArgs : EventArgs
    {

        /// <summary>
        /// OPC具体类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 读取到的OPC数据
        /// </summary>
        public Hashtable DataChangedResult { get; set; }

        /// <summary>
        /// OPC数据读取错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 初始化一个OPCDataChangedEventArgs实例
        /// </summary>
        public OpcDataChangedEventArgs() { }

        /// <summary>
        /// 以读取到的数据集初始化一个OPCDataChangedEventArgs实例
        /// </summary>
        /// <param name="type">OPC具体类型</param>
        /// <param name="opcResult">已转换为Hashtable的数据集合，Tag:Vale形式</param>
        public OpcDataChangedEventArgs(string type,Hashtable opcResult)
        {
            Type = type;
            DataChangedResult = opcResult;
        }

        /// <summary>
        /// 以读取到的数据集和错误信息初始化一个OPCDataChangedEventArgs实例
        /// </summary>
        /// <param name="type">OPC具体类型</param>
        /// <param name="opcResult">已转换为Hashtable的数据集合，Tag:Vale形式</param>
        /// <param name="errorMessage">错误信息</param>        
        public OpcDataChangedEventArgs(string type,Hashtable opcResult, string errorMessage)
            : this(type,opcResult)
        {
            ErrorMessage = errorMessage;
        }

    }

    /// <summary>
    /// 模块：OPC Core
    /// 摘要：定义OPCDataChangedEventArgs的扩展方法
    /// 作者：Tsccai
    /// 编写日期：2013/3/25 22:43:24
    /// </summary>
    public static class OPCDataChangedEventArgsEx
    {
        /// <summary>
        /// 将Hashtable形式的数据集转换为JSON格式
        /// </summary>
        /// <param name="h">Hashtable形式的数据集</param>
        /// <returns>JSON格式的数据</returns>
        public static string ToJSON(this Hashtable h)
        {
            string inner = "";

            string content = "{";
            foreach (DictionaryEntry item in h)
            {
                content += @"""" + item.Key + @""":""" + item.Value + @""",";
            }
            content = content.Substring(0, content.Length - 1) + "},";
            inner += content;

            inner = inner.Substring(0, inner.Length - 1);
            string result = @"{""d"":[" + inner + "]}";
            return result;
        }
    }
}
