using System;
using System.Collections;
using Tsclab.OpcAccess.Core;

namespace Tsclab.OpcAccess.QuickStart
{
    /// <summary>
    /// 模块：OpcListener
    /// 摘要：定制本系统所需的OpcListener
    /// 作者：Tsccai
    /// 编写日期：2012/2/3 19:28:38
    /// </summary>
    public class PhListener : Tsclab.OpcAccess.Core.OpcListener
    {
        public PhListener(OpcConfig config) : base(config) { }

        protected override Hashtable DoExtraProcess(Hashtable h)
        {
            string key;
            decimal value;
            if (h.ContainsKey("01_FaDian"))
            {
                key = "01_YongDianLv";
                decimal su = Convert.ToDecimal(h["01_FaDian"]);
                value = Decimal.Round(GpRawDataProcessor.CalculateSCLv(su) * 100, 3);
                h.Add(key, value);
            }
            if (h.ContainsKey("02_FaDian"))
            {
                key = "02_YongDianLv";
                decimal su = Convert.ToDecimal(h["02_FaDian"]);
                value =Decimal.Round(GpRawDataProcessor.CalculateSCLv(su) * 100, 3);
                h.Add(key,value);
            }

            if (h.ContainsKey("03_FaDian") && h.ContainsKey("03_YongDian"))
            {
                key = "03_YongDianLv";
                decimal su = Convert.ToDecimal(h["03_FaDian"]);
                decimal sc = Convert.ToDecimal(h["03_YongDian"]);
                value =Decimal.Round(GpRawDataProcessor.CalculateSCLv(su, sc) * 100, 3);
                h.Remove("03_YongDian");
                h.Add(key, value);
            }

            if (h.ContainsKey("04_FaDian") && h.ContainsKey("04_YongDian"))
            {
                key = "04_YongDianLv";
                decimal su = Convert.ToDecimal(h["04_FaDian"]);
                decimal sc = Convert.ToDecimal(h["04_YongDian"]);
                value = Decimal.Round(GpRawDataProcessor.CalculateSCLv(su, sc) * 100, 3);
                h.Remove("04_YongDian");
                h.Add(key, value);
            }

            return h;
        }
    }
}
