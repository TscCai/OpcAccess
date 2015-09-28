using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC Core
    /// 摘要：定义OPCListener接口
    /// 作者：Tsccai
    /// 时间：2012/2/2 10:14:34
    /// </summary>

    public interface IOpcListener
    {
        /// <summary>
        /// 当前是否与OPCServer连接
        /// </summary>
        bool Started { get;}

        /// <summary>
        /// OPC数据变动事件
        /// </summary>
        event OpcDataChangedEventHandler OPCDataChanged;

        /// <summary>
        /// 启动监听器（将自动连接到服务器）
        /// </summary>
        void Start();

        /// <summary>
        /// 关闭与OPC服务器的连接并释放资源
        /// </summary>
        void Close();

    }

 


}
