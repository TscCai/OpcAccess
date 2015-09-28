using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC数据读取
    /// 摘要：解析OPC配置文件，默认解析根目录下Tsclab.OpcAccess.config
    /// 作者：Tsccai
    /// 编写日期：2011/12/23 20:31:37
    /// </summary>
    public class OpcConfigManager
    {
        private OpcConfig result;
        private XElement configXml;
        private IEnumerable<XElement> opcInfos;
        private string filename;
        private string type;

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public OpcConfigManager() { }

        /// <summary>
        /// 带参构造方法
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <param name="type">配置类型(参考Tsclab.OpcAccess.config)</param>
        public OpcConfigManager(string filename, string type)
        {
            this.filename = filename;
            this.type = type;
        }

        #region 解析配置信息的具体方法

        /// <summary>
        /// 读取IP
        /// </summary>
        private void GetIP()
        {
            var ip = (from xml in opcInfos.Descendants("ip") select xml).Single();
            result.IP = ip.Value;

        }

        /// <summary>
        /// 读取服务器名
        /// </summary>
        private void GetServerName()
        {
            var serverName = (from xml in opcInfos.Descendants("serverName") select xml).Single();

            result.ServerName = serverName.Value;

        }

        /// <summary>
        /// 读取Group
        /// </summary>
        private void GetGroup()
        {
            var group = (from xml in opcInfos.Descendants("group") select xml).Single();

            result.Group.Name = group.Attribute((XName)"name").Value;

            GetTimespan(group);
            GetGroupDefaultProperties(group);
            GetGroupProperties(group);
            GetItems(group);
        }

        /// <summary>
        /// 读取时间间隔
        /// </summary>
        /// <param name="group"></param>
        private void GetTimespan(XElement group)
        {
            var timespan = group.Attribute((XName)"timespan");
            result.Group.Timespan = Convert.ToInt32(timespan.Value);
        }

        /// <summary>
        /// 读取Group默认设置
        /// </summary>
        /// <param name="group"></param>
        private void GetGroupDefaultProperties(XElement group)
        {
            var defaultGroupProperties = (from xml in @group.Descendants("groupDefaultProperties") select xml).Single();

            result.Group.GroupDefaultProperties.IsActive = Convert.ToBoolean(defaultGroupProperties.Attribute((XName)"isActive").Value);
            result.Group.GroupDefaultProperties.DeadBand = Convert.ToInt32(defaultGroupProperties.Attribute((XName)"deadBand").Value);
            result.Group.GroupDefaultProperties.UpdateRate = Convert.ToInt32(defaultGroupProperties.Attribute((XName)"updateRate").Value);
        }

        /// <summary>
        /// 读取Group设置
        /// </summary>
        /// <param name="group"></param>
        private void GetGroupProperties(XElement group)
        {
            var groupProperties = (from xml in @group.Descendants("groupProperties") select xml).Single();

            result.Group.GroupProperties.IsActive = Convert.ToBoolean(groupProperties.Attribute((XName)"isActive").Value);
            result.Group.GroupProperties.DeadBand = Convert.ToInt32(groupProperties.Attribute((XName)"deadBand").Value);
            result.Group.GroupProperties.UpdateRate = Convert.ToInt32(groupProperties.Attribute((XName)"updateRate").Value);
            result.Group.GroupProperties.IsSubscribed = Convert.ToBoolean(groupProperties.Attribute((XName)"isSubscribed").Value);

        }

        /// <summary>
        /// 读取Items
        /// </summary>
        /// <param name="group"></param>
        private void GetItems(XElement group)
        {
            var opcItems = from xml in @group.Descendants("opcItem") select xml;
            result.Group.OpcItems = new OpcItem[opcItems.Count()];
            GetItems(opcItems);
        }

        /// <summary>
        /// 按Group读取Item
        /// </summary>
        /// <param name="opcItems"></param>
        private void GetItems(IEnumerable<XElement> opcItems)
        {
            int jzIndex = 0;
            foreach (var item in opcItems)
            {
                result.Group.OpcItems[jzIndex].UnitId = Convert.ToInt32(item.Attribute((XName)"id").Value);
                result.Group.OpcItems[jzIndex].Items = new Item[item.Descendants("item").Count()];

                GetItemByUnitIndex(item, jzIndex);
                //itemIndex++;
                jzIndex++;
            }
        }

        /// <summary>
        /// 按机组索引获取其Items
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        private void GetItemByUnitIndex(XElement item, int index)
        {
            int itemIndex = 0;
            var items = from xml in item.Descendants("item") select xml;
            foreach (var subItem in items)
            {

                result.Group.OpcItems[index].Items[itemIndex].SubItems = new Dictionary<string, string>();

                string name = subItem.Attribute((XName)"name").Value;
                result.Group.OpcItems[index].Items[itemIndex].Name = name;
                if (subItem.Attribute("strategy") != null && subItem.Attribute("strategy").Value.ToLower() != "single")
                {
                    string strategy = subItem.Attribute((XName)"strategy").Value;
                    strategy = strategy[0].ToString().ToUpper() + strategy.Substring(1);
                    result.Group.OpcItems[index].Items[itemIndex].Strategy =strategy;
                    var opcItem = from subXml in subItem.Descendants("subItem") select subXml;

                    foreach (var inner in opcItem)
                    {
                        string tagName = inner.Attribute((XName)"name").Value;
                        string itemID = inner.Value;
                        result.Group.OpcItems[index].Items[itemIndex].SubItems.Add(tagName, itemID);
                    }
                }
                else
                {
                    string itemID = subItem.Value;
                    result.Group.OpcItems[index].Items[itemIndex].SubItems.Add(name, itemID);
                    result.Group.OpcItems[index].Items[itemIndex].Strategy = "Single";
                }
                itemIndex++;
            }
        }

        private void GetRawDataProcessor()
        {
            try
            {
                var processors = (from xml in configXml.Descendants("rawDataProcessor") select xml);
                //one more for default processor.
                result.RawDataProcessor = new RawDataProcessor[processors.Elements().Count()+1];
                //Add default processor at first.
                InitProcessor(result.RawDataProcessor);
                int i = 1;
                //Add customer processor.
                foreach (XElement item in processors.Elements())
                {
                    result.RawDataProcessor[i].Assembly = item.Attribute((XName)"assembly").Value;
                    result.RawDataProcessor[i].Fullname = item.Attribute((XName)"fullname").Value;
                    i++;
                }

            }
            catch(Exception ex)
            {
                //When <RawDataProcessor/> node is missing or only has a empty <Class/> childnode, the following
                //exceptions will be raised, just use the default processor.
                if (ex is InvalidOperationException || ex is NullReferenceException)
                {
                    InitProcessor(result.RawDataProcessor);
                }
                else
                {
                    throw ex;
                }
            }

        }

        private void InitProcessor(RawDataProcessor[] rdp)
        {
            if (rdp == null)
            {
                rdp = new RawDataProcessor[1];
            }
            rdp[0].Assembly = "Tsclab.OpcAccess";
            rdp[0].Fullname = "Tsclab.OpcAccess.DefaultRawDataProcessor";
        }

        #endregion

        /// <summary>
        /// 进行配置文件解析
        /// </summary>
        /// <returns>包含配置信息的OPCConfig</returns>
        private OpcConfig Configure()
        {
            configXml = XElement.Load(filename);
            opcInfos = from xml in configXml.Descendants("opcInfo").Where(t => t.Attribute((XName)"type").Value == type) select xml;
            result = new OpcConfig();
            result.Type = type;
            GetIP();
            GetServerName();
            GetGroup();
            GetRawDataProcessor();
            return result;
        }

        /// <summary>
        /// 获取完成解析的OPC配置信息
        /// </summary>
        /// <param name="filename">配置文件名</param>
        /// <param name="type">配置类型(参考Tsclab.OpcAccess.config)</param>
        /// <returns>包含配置信息的OPCConfig</returns>
        public static OpcConfig Configure(string filename, string type)
        {
            OpcConfigManager opcCM = new OpcConfigManager(filename, type);
            return opcCM.Configure();
        }

        /// <summary>
        /// 获取完成解析的OPC配置信息
        /// </summary>
        /// <param name="type">配置类型(参考Tsclab.OpcAccess.config)</param>
        /// <returns>包含配置信息的OPCConfig</returns>
        public static OpcConfig Configure(string type)
        {

            string filename = AppDomain.CurrentDomain.BaseDirectory + @"\Tsclab.OpcAccess.config";
            OpcConfigManager opcCM = new OpcConfigManager(filename, type);
            return opcCM.Configure();
        }

    }




}
