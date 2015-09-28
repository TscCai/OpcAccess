using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Tsclab.OpcAccess.Core;
using System.Reflection;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC Core
    /// 摘要：IOpcListener接口的默认实现类
    /// 作者：Tsccai
    /// 版本：1.0
    /// 编写日期：2012/2/2 19:33:10
    /// 
    /// 版本：1.1
    /// 备注：修正了某些数据没有更新时返回0值的bug，现可保留上一次读取到的值。
    ///               将Dictionary<string,string>clientHandles成员变量的名称改为rowIdMap
    ///               将ItemRow.Data由List<object>改为object[]
    ///               ItemRow增加Timestamp属性，存在反复赋值但只最后一次有效
    ///               且目前还没有被使用过
    ///               将ItemRow原公共变量修改为属性，添加构造方法
    /// 作者：Tsccai
    /// 编写日期：2013/10/25 20:29:34
    /// </summary>
    public class OpcListener : OpcReader, IOpcListener
    {

        int errorCount = 0;
        List<string> errorItems = new List<string>();
        List<string> itemIDs = new List<string>();
        Hashtable rowIdMap = new Hashtable();
        List<ItemRow> itemTable = new List<ItemRow>();
        /// <summary>
        /// OPCListener是否已启动
        /// </summary>
        public bool Started { get { return base.Connected; } }
        /// <summary>
        /// OPC数据变动事件
        /// </summary>
        public virtual event OpcDataChangedEventHandler OPCDataChanged;

        /// <summary>
        /// 使用OPCConfig来初始化新的OPCListener实例
        /// </summary>
        /// <param name="config"></param>
        public OpcListener(OpcConfig config)
            : base(config)
        {

        }

        /// <summary>
        /// 按OPCConfig中的配置信息，以订阅方式监听OPC数据的变化
        /// </summary>
        public virtual void Start()
        {
            if (!Started)
            {
                base.Connect();
            }
            int[] itemHandles = SetupItems();
            AddItems(itemIDs.ToArray(), itemHandles, true);
            Group.DataChange += new OPCAutomation.DIOPCGroupEvent_DataChangeEventHandler(Group_DataChange);
        }

        private void Group_DataChange(int _TransactionID, int _NumItems, ref Array _ClientHandles, ref Array _ItemValues, ref Array _Qualities, ref Array _timeStamps)
        {
            //_ClientHandles可确定该条数据是哪个测点的
           int id=0;
            for (int i = 1; i <= _NumItems; i++)
            {
                OPCAutomation.OPCQuality quality = (OPCAutomation.OPCQuality)_Qualities.GetValue(i);
                int clientHandle = (int)_ClientHandles.GetValue(i);
                id = (int)rowIdMap[clientHandle];
                     
                if (quality == OPCAutomation.OPCQuality.OPCQualityGood)
                {
                    object value = _ItemValues.GetValue(i);
                    ItemRow ir = itemTable[id-1];
                    //clientHandle-id可保证每次都覆盖clientHandle对应位置上的数据并保留本次没有更新到的数据
                    ir.Data[clientHandle-id] = value;

                    //重复赋值，但只取最后一次的值，待优化
                    ir.Timestamp = (DateTime)_timeStamps.GetValue(i);
                }
                else
                {
                    errorCount++;
                    errorItems.Add(itemIDs[id - 1]);
                }
            }
            
            Hashtable result = ProcessItemTable();

            if (errorCount == 0)
            {
                OPCDataChanged(this, new OpcDataChangedEventArgs(Config.Type,result));
            }
            else
            {
                string message = OpcUnableToReadException.CreateErrorMessage(errorCount, errorItems);
                OPCDataChanged(this, new OpcDataChangedEventArgs(Config.Type,result, message));
            }
        }

        /// <summary>
        /// 按Item处理策略对Item的数据进行处理
        /// </summary>
        /// <returns></returns>
        protected virtual Hashtable ProcessItemTable()
        {
            Hashtable result = new Hashtable();
            string key;
            object value = 0;
            foreach (ItemRow ir in itemTable)
            {
                key = ir.Name;
                object[] raw;
                raw = ir.Data.ToArray();
                value = RunProcessorMethod(true, ir.Strategy, raw);
                result.Add(key, value);
            }
            result.Add("Timestamp", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffff"));
            try
            {
                return DoExtraProcess(result);
            }
            catch(Exception ex)
            {
                return result;
            }
        }

        /// <summary>
        /// call outer method through reflection to deal with the raw data.
        /// </summary>
        /// <param name="ignoreCase">Ignore the case of the method name</param>
        /// <param name="strategy">the deal strategy in config file, equals the method name</param>
        /// <param name="data">the raw data to deal</param>
        /// <returns>dealed data</returns>
        protected virtual object RunProcessorMethod(bool ignoreCase, string strategy, object[] data)
        {
            object result = null;
            foreach (RawDataProcessor item in Config.RawDataProcessor)
            {
                try
                {
                    Assembly assembly = Assembly.Load(item.Assembly);
                    Type processor = assembly.GetType(item.Fullname);
                    BindingFlags bf = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;
                    if (ignoreCase)
                    {
                        bf = bf | BindingFlags.IgnoreCase;
                    }

                    //优先查找返回值为object的方法
                    MethodInfo method = processor.GetMethod(
                        strategy,
                        bf,
                        null,
                        new Type[] { typeof(object[]) },
                        null
                        );
                    if (method == null)
                    {
                        method = processor.GetMethod(strategy, bf);
                    }
                    result = method.Invoke(null, new object[] { data });
                    break;
                }
                catch
                {
                    result = "Failed to process.";
                    continue;
                }
            }
            return result;
        }

        /// <summary>
        /// 对采集到的数据进行额外处理、过滤等操作
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        protected virtual Hashtable DoExtraProcess(Hashtable h)
        {
            return h;
        }

        /// <summary>
        /// 为待监听的全部Item生成ClientHandles，
        /// 根据配置信息将这些ClientHandles归档(Sum,Avg,Single)，
        /// 并返回为之生成的ClientHandles
        /// </summary>
        /// <returns>生成的ClientHandles</returns>
        private int[] SetupItems()
        {
            int handle = 1;
            int id = 1;
            List<int> result = new List<int>();
            for (int i = 0; i < Config.Group.OpcItems.Length; i++)
            {
                foreach (var item in Config.Group.OpcItems[i].Items)
                {
                    ItemRow ir = new ItemRow();
                    ir.ID = id;
                    ir.Name = item.Name;
                    ir.Strategy = item.Strategy;
                    ir.Handles = new List<int>();
                    ir.Data = new object[item.SubItems.Count];
                    foreach (var subItem in item.SubItems)
                    {
                        rowIdMap.Add(handle, id);
                        ir.Handles.Add(handle);
                        result.Add(handle);
                        itemIDs.Add(subItem.Value);
                        handle++;
                    }
                    itemTable.Add(ir);
                    id++;
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 取消OPC事件订阅，并断开与OPC服务器的连接
        /// </summary>
        public new void Close()
        {
            if (this != null && Group != null)
            {
                Group.DataChange -= new OPCAutomation.DIOPCGroupEvent_DataChangeEventHandler(Group_DataChange);
                base.Close();

            }
        }

        /// <summary>
        /// 当本实例被GC清除时自动释放OPC服务器的连接
        /// </summary>
        ~OpcListener()
        {
            this.Close();
        }
    }

    /// <summary>
    /// 构建Listener中ItemTable的行元素
    /// </summary>
    class ItemRow
    {
        public ItemRow()
        {
            Strategy = "Single";
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Strategy { get; set; }
        public List<int> Handles { get; set; }
        public object[] Data{get;set;}
        public DateTime Timestamp { get; set; }
    }

}
