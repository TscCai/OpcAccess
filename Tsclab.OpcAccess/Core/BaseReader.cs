using System;
using System.Collections.Generic;
using OPCAutomation;

namespace Tsclab.OpcAccess.Core
{
    /// <summary>
    /// 模块：OPC数据读取
    /// 摘要：OPCReader的内部核心
    /// 作者：Tsccai
    /// 编写日期：2011.12.13
    /// </summary>
    public class BaseReader
    {
        #region 定义OPC对象

        /// <summary>
        /// OPC服务器
        /// </summary>
        public OPCServer Server { get; set; }
        
        /// <summary>
        /// 该服务器中的全部OPCGroups
        /// </summary>
        public OPCGroups Groups { get; set; }

        /// <summary>
        /// 单个OPCGroup
        /// </summary>
        public OPCGroup Group { get; set; }

        /// <summary>
        /// OPCGroup中的Items
        /// </summary>
        public OPCItems Items { get; set; }

        #endregion


        #region 定义Reader属性

        /// <summary>
        /// 主机IP
        /// </summary>
        public string HostIP { get; set; }

        /// <summary>
        /// 主机名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// 客户端句柄
        /// </summary>
        public int ItemClientHandle { get; set; }

        
        Array itemServerHandles;
        /// <summary>
        /// 服务器端句柄
        /// </summary>
        public Array ItemServerHandles { get { return itemServerHandles; } set { itemServerHandles = value; } }
        
        #endregion

        /// <summary>
        /// 初始化BaseReader实例
        /// </summary>
        public BaseReader()
        {
            this.Server = new OPCServer();
        }

        /// <summary>
        /// 以指定的IP和ServerName初始化BaseReader
        /// </summary>
        /// <param name="hostIP"></param>
        /// <param name="serverName"></param>
        public BaseReader(string hostIP, string serverName)
            : this()
        {
            this.HostIP = hostIP;
            this.ServerName = serverName;
        }

        /// <summary>
        /// 以OPC-DA组件连接到OPC服务器
        /// <excpetion cref="Exception"/>
        /// </summary>
        public void Connect()
        {
            bool result = false;
            try
            {
                Server.Connect(ServerName, HostIP);
                if (Server.ServerState == (int)OPCServerState.OPCRunning)
                {
                    result = true;
                }
                Connected = result;
            }
            catch(Exception ex)
            {
                throw new ServerUnableToConnectException(ex.Message);
            }

        }

        /// <summary>
        /// 添加Group的默认属性
        /// </summary>
        /// <param name="isActive">是否激活</param>
        /// <param name="deadband">死区</param>
        /// <param name="updateRate">更新频率</param>
        protected void SetGroupDefaultProperty(bool isActive, int deadband, int updateRate)
        {
            Server.OPCGroups.DefaultGroupIsActive = isActive;
            Server.OPCGroups.DefaultGroupDeadband = deadband;
            Server.OPCGroups.DefaultGroupUpdateRate = updateRate;
        }

        /// <summary>
        /// 添加Group属性
        /// </summary>
        /// <param name="isActive">是否激活</param>
        /// <param name="deadband">死区</param>
        /// <param name="updateRate">更新频率</param>
        /// <param name="isSubscribed">是否订阅Group事件(订阅式/异步读写时必须为true)</param>
        protected void SetGroupProperty(bool isActive, int deadband, int updateRate, bool isSubscribed)
        {
            Group.DeadBand = deadband;
            Group.UpdateRate = updateRate;
            Group.IsActive = isActive;
            Group.IsSubscribed = isSubscribed;
        }

        /// <summary>
        /// 移除所有Items
        /// </summary>
        protected void RemoveAllItems()
        {
            if (ItemClientHandle != 0)
            {
                Array errors;
                OPCItem opcItem;
                List<int> temp = new List<int>();
                temp.Add(0);
                foreach (object item in ItemServerHandles)
                {
                    try
                    {
                        opcItem = Items.GetOPCItem((int)item);
                        temp.Add(opcItem.ServerHandle);
                    }
                    catch
                    {
                        continue;
                    }
                }

                //注：OPC中以1为数组的索引起始
                if (temp.Count > 0)
                {
                    Array serverHandle = temp.ToArray();
                    //移除上一次选择的项
                    Items.Remove(Items.Count, ref serverHandle, out errors);
                }
            }
        }

        /// <summary>
        /// 添加OPCItems
        /// </summary>
        /// <param name="itemNames">待添加的OPCItems名</param>
        /// <param name="itemClientHandles">客户端句柄</param>
        /// <param name="removeFormer">是否移除当前已存在的Items</param>
        protected void AddItems(string[] itemNames, int[] itemClientHandles, bool removeFormer)
        {
            try
            {
                if (removeFormer)
                {
                    RemoveAllItems();
                }
                Array ar_itemNames = Array.CreateInstance(typeof(string), itemNames.Length + 1);
                ((Array)itemNames).CopyTo(ar_itemNames, 1);
                Array ar_itemClientHandles = Array.CreateInstance(typeof(int), itemNames.Length + 1);
                ((Array)itemClientHandles).CopyTo(ar_itemClientHandles, 1);
                Array ar_Errors;

                Items.AddItems(ar_itemClientHandles.Length - 1, ref ar_itemNames, ref ar_itemClientHandles, out itemServerHandles, out ar_Errors);
            }
            catch
            {
               //没有任何权限的项，都是OPC服务器保留的系统项，此处可不做处理。
            }
        }

        /// <summary>
        /// 异步读取数据，异步读取时需添加响应委托
        /// </summary>
        protected void AsyncRead()
        {
            Array handles = ItemServerHandles;
            Array errors;
            int cancleID;
            //以下方法中的TransactionID的值为任意给出，其意义不明。
            Group.AsyncRead(ItemServerHandles.Length - 1, ref handles, out errors, 2011, out cancleID);
            GC.Collect();
        }

        /// <summary>
        /// 同步读取数据
        /// </summary>
        /// <param name="values">读取到的OPC数据</param>
        /// <param name="qualities">对应OPC数据的质量</param>
        /// <param name="timeStamps">对应OPC数据读取的时间戳</param>
        protected void SyncRead(out Array values, out object qualities, out object timeStamps)
        {
            Array handles = ItemServerHandles;
            Array errors;
            Group.SyncRead(1, ItemServerHandles.Length, ref handles, out values, out errors, out qualities, out timeStamps);
            GC.Collect();
        }

        /// <summary>
        /// 断开与OPCServer的连接
        /// </summary>
        public void Close()
        {
            if (this.Server != null)
            {
                try
                {
                    RemoveAllItems();
                    this.Groups.RemoveAll();
                }
                catch { }
                finally
                {
                    this.Server.Disconnect();
                    this.Server = null;
                }
            }
            this.Connected = false;
        }

        /// <summary>
        /// 对象被GC销毁时，释放与OPC服务器的连接
        /// </summary>
        ~BaseReader()
        {
            Close();
        }

    }
}
