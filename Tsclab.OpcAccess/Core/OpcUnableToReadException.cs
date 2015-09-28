using System;
using System.Collections;
using System.Collections.Generic;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC Core
    /// 摘要：当OPC无法读取时抛出该异常
    /// 作者：Tsccai
    /// 编写日期：2012.01.07
    /// </summary>
    public class OpcUnableToReadException:ApplicationException
    {
        /// <summary>
        /// 余留的、正常读取到的结果；异常的值将以0代替
        /// </summary>
        public Hashtable RemainResult { get; set; }

         /// <summary>
        /// OPCUnableToReadException的默认构造方法
        /// </summary>
        public OpcUnableToReadException() : base() { }

        /// <summary>
        /// OPCUnableToReadException的带有错误信息的构造方法
        /// </summary>
        /// <param name="message"></param>
        public OpcUnableToReadException(string message) : base(message) { }

        /// <summary>
        /// 带有错误信息和余留结果的构造方法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="remainResult"></param>
        public OpcUnableToReadException(string message, Hashtable remainResult):base(message)
        {
            RemainResult = remainResult;
        }

        /// <summary>
        /// 生成错误信息
        /// </summary>
        /// <param name="errorCount"></param>
        /// <param name="errorItems"></param>
        /// <returns></returns>
        public static string CreateErrorMessage(int errorCount, List<string> errorItems)
        {
            string message = "以下" + errorCount + "个OPC参数未能成功读取：\n";
            foreach (string item in errorItems)
            {
                message += item + ",";
            }
            message = message.Substring(0, message.Length - 1);
            return message;
        }

        /// <summary>
        /// 生成错误信息
        /// </summary>
        /// <param name="errorCount"></param>
        /// <param name="errorItems"></param>
        /// <returns></returns>
        public static string GenerateErrorMessage(int errorCount, string[] errorItems)
        {
            string message = "以下" + errorCount + "个OPC参数未能成功读取：\n";
            foreach (string item in errorItems)
            {
                message += item + ",";
            }
            message = message.Substring(0, message.Length - 1);
            return message;
        }

    }
}
