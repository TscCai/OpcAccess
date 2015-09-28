using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC Core
    /// 摘要：当OPC服务器无法连接时抛出该异常
    /// 作者：Tsccai
    /// 编写日期：2013.04.14
    /// </summary>
    public class ServerUnableToConnectException:ApplicationException
    {
        /// <summary>
        /// 构造一个ServerUnableToConnectException实例
        /// </summary>
        public ServerUnableToConnectException() : base() { }

        /// <summary>
        /// 构造一个ServerUnableToConnectException实例 
        /// </summary>
        /// <param name="message">错误信息</param>
        public ServerUnableToConnectException(string message): base(message){}
    }
}
