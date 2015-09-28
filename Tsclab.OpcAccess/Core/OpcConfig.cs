using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 定义OPC数据的相关配置信息
    /// </summary>
    public struct OpcConfig
    {
        /// <summary>
        /// OPC具体类型
        /// </summary>
        public string Type;

        /// <summary>
        /// OPCServer的IP地址
        /// </summary>
        public string IP;

        /// <summary>
        ///OPC服务器名，非远程计算机名
        /// </summary>
        public string ServerName;

        /// <summary>
        /// OPCGroup
        /// </summary>
        public OpcGroup Group;

        /// <summary>
        /// Raw data processor
        /// </summary>
        public RawDataProcessor[] RawDataProcessor;
    }


    /// <summary>
    /// OPCGroup
    /// </summary>
    public struct OpcGroup
    {
        /// <summary>
        /// Group名
        /// </summary>
        public string Name;

        /// <summary>
        /// 每次读取间隔时间，单位：毫秒
        /// 实际应用中，两次读取时间间隔过短会导致数据无法顺利读取
        /// 设置读取时间间隔的目的在于解决此问题
        /// 应设置在50ms~500ms的合理范围内
        /// </summary>
        public int Timespan;

        /// <summary>
        /// Group默认属性
        /// </summary>
        public GroupDefaultProperty GroupDefaultProperties;

        /// <summary>
        /// Group属性
        /// </summary>
        public GroupProperty GroupProperties;

        /// <summary>
        /// OPCItems
        /// </summary>
        public OpcItem[] OpcItems;
    }


    /// <summary>
    /// Group默认属性
    /// </summary>
    public struct GroupDefaultProperty
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// 死区
        /// </summary>
        public int DeadBand;

        /// <summary>
        /// 更新频率
        /// </summary>
        public int UpdateRate;
    }


    /// <summary>
    /// Group属性
    /// </summary>
    public struct GroupProperty
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// 死区
        /// </summary>
        public int DeadBand;

        /// <summary>
        /// 更新频率
        /// </summary>
        public int UpdateRate;

        /// <summary>
        /// 是否订阅事件，异步读写时必须为true
        /// </summary>
        public bool IsSubscribed;
    }


    /// <summary>
    /// OPCItem
    /// </summary>
    public struct OpcItem
    {
        /// <summary>
        /// 机组编号
        /// </summary>
        public int UnitId;

        /// <summary>
        /// 该功能、该机组所涉及的Item
        /// </summary>
        public Item[] Items;
    }

    /// <summary>
    /// Item
    /// </summary>
    public struct Item
    {
        /// <summary>
        /// 数据处理策略
        /// </summary>
        public string Strategy;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 与OPCServer中对应的Item
        /// Key:TagName
        /// Value:ItemID
        /// </summary>
        public Dictionary<string, string> SubItems;
    }

    /// <summary>
    /// 数据处理策略
    /// </summary>
    //public enum ItemStrategy
    //{
    //    /// <summary>
    //    /// 单个Item，不做任何处理
    //    /// </summary>
    //    Single,

    //    /// <summary>
    //    /// 将该组Items求平均值
    //    /// </summary>
    //    Average,

    //    /// <summary>
    //    /// 将该组Items求和
    //    /// </summary>
    //    Sum
    //}

    /// <summary>
    /// 数据处理器
    /// </summary>
    public struct RawDataProcessor
    {
        /// <summary>
        /// 程序集
        /// </summary>
        public string Assembly;

        /// <summary>
        /// 类的全名
        /// </summary>
        public string Fullname;
    }
}
