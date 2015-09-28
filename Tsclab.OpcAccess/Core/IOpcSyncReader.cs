using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC Core
    /// 摘要：定义读取接口
    /// 作者：Tsccai
    /// 编写日期：2012/1/29 15:57:55
    /// </summary>
    public interface IOpcSyncReader
    {
        /// <summary>
        /// 读取OPC数据
        /// </summary>
        /// <param name="jzid">机组编号</param>
        /// <returns>读取结果集</returns>
        Hashtable Read(int jzid);

        /// <summary>
        /// 指示是否已与OPC服务器建立连接
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// 连接OPC服务器
        /// </summary>
        void Connect();

        /// <summary>
        /// 断开与OPC服务器的连接
        /// </summary>
        void DisConnect();
    }

}
