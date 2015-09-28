using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsclab.OpcAccess.Core;
using System.Collections;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC Core
    /// 摘要：实现了从配置信息创建OpcReader实例
    /// 作者：Tsccai
    /// 编写日期：2012/1/29 16:09:22
    /// </summary>
    public class OpcReader:BaseReader
    {
        /// <summary>
        /// OPC配置信息
        /// </summary>
        public OpcConfig Config { get; set; }

        /// <summary>
        /// 初始化SZOPCReader实例
        /// </summary>
        public OpcReader() { }

        /// <summary>
        /// 以OPCConfig初始化SZOPCReader实例
        /// </summary>
        /// <param name="config"></param>
        public OpcReader(OpcConfig config)
        {
            //Init
            Config = config;
            //Connect
            base.HostIP = Config.IP;
            base.ServerName = Config.ServerName;
          
        }

        /// <summary>
        /// 以OPC-DA组件连接到OPC服务器，
        /// 添加Group并设置Group的各种属性
        /// </summary>
        public new void Connect()
        {
            base.Connect();
            //Settings
            base.Groups = base.Server.OPCGroups;
            base.Group = base.Groups.Add(Config.Group.Name);
            GroupDefaultProperty groupDefaultProperty = Config.Group.GroupDefaultProperties;
            base.SetGroupDefaultProperty(groupDefaultProperty.IsActive, groupDefaultProperty.DeadBand, groupDefaultProperty.UpdateRate);
            GroupProperty groupProperty = Config.Group.GroupProperties;
            base.SetGroupProperty(groupProperty.IsActive, groupProperty.DeadBand, groupProperty.UpdateRate, groupProperty.IsSubscribed);
            base.Items = base.Group.OPCItems;
        }

       
    }
}
