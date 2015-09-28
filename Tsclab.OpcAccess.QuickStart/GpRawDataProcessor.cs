using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsclab.OpcAccess.Core;

namespace Tsclab.OpcAccess.QuickStart
{
    /// <summary>
    /// 模块：OPC数据处理
    /// 摘要：将读取到的OPC数据处理为网页显示的模式
    /// 作者：Tsccai
    /// 编写日期：2011/12/26 15:12:52
    /// </summary>
    public class GpRawDataProcessor
    {
        /// <summary>
        /// 计算厂用电量。A厂煤耗计算专用
        /// </summary>
        /// <param name="su">厂发电量</param>
        /// <returns></returns>
        public static decimal CalculateSC(decimal su)
        {
            decimal result = CalculateSCLv(su) * su;
            return result;
        }

        /// <summary>
        /// 计算厂用电率，A厂正平衡计算专用，小数表示
        /// </summary>
        /// <param name="su">厂发电量</param>
        /// <returns>返回值为小数</returns>
        public static decimal CalculateSCLv(decimal su)
        {
            decimal result = (0.000056m * su * su - 0.087698m * su + 38.554805m + 1.9m / 100) / 100;
            return result;
        }

        /// <summary>
        /// 计算厂用电率，B厂正平衡计算专用，小数表示
        /// </summary>
        /// <param name="sc">厂用电量</param>
        /// <param name="su">厂发电量</param>
        /// <returns>返回值为小数</returns>
        public static decimal CalculateSCLv(decimal su, decimal sc)
        {
            decimal result = sc / su;
            return result;
        }
    }
}
