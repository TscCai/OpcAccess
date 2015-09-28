using System;
namespace Tsclab.OpcAccess
{
    /// <summary>
    /// 模块：OPC数据读取
    /// 摘要：将读取到的OPC数据处理为网页显示的模式
    /// 作者：Tsccai
    /// 编写日期：2011/12/26 15:12:52
    /// </summary>
    public sealed class DefaultRawDataProcessor
    {
        /// <summary>
        /// 求平均
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal Average(object[] values)
        {
            return Average(ToDecimal(values));
        }

        /// <summary>
        /// 求平均
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal Average(decimal[] values)
        {
            decimal result = Sum(values) / values.Length;
            return result;
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal Sum(object[] values)
        {
            return Sum(ToDecimal(values));
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal Sum(decimal[] values)
        {
            decimal result = 0m;
            foreach (decimal item in values)
            {
                result += item;
            }
            return result;
        }

        /// <summary>
        /// 不做处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Single(object[] value)
        {
            return value[0];
        }

        /// <summary>
        /// 将object数组转换为Decimal数组
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static decimal[] ToDecimal(object[] values)
        {
            decimal[] result = new decimal[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = Convert.ToDecimal(values[i]);
            }
            return result;
        }
    }

}
