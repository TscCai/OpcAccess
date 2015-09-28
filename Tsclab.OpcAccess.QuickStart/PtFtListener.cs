using System;
using System.Collections;
using Tsclab.OpcAccess.Core;


namespace Tsclab.OpcAccess.QuickStart
{
    /// <summary>
    /// 模块：OpcListener
    /// 摘要：定制本系统所需的OpcListener
    /// 作者：Tsccai
    /// 编写日期：2012/2/4 11:42:25
    /// </summary>
    public class PtFtListener : Tsclab.OpcAccess.Core.OpcListener
    {
        public PtFtListener(OpcConfig config) : base(config) { }

        protected override Hashtable DoExtraProcess(Hashtable h)
        {
            if (h.ContainsKey("01_SU"))
            {
                decimal su = Convert.ToDecimal(h["01_SU"]);
                decimal sc =Decimal.Round(GpRawDataProcessor.CalculateSC(su), 3);
                h.Add("01_SC", sc);
            }
            if (h.ContainsKey("02_SU"))
            {
                decimal su = Convert.ToDecimal(h["02_SU"]);
                decimal sc = Decimal.Round(GpRawDataProcessor.CalculateSC(su), 3);
                h.Add("02_SC", sc);
            }

            return h;
        }
    }
}
