using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsclab.OpcAccess.Core;

namespace Tsclab.OpcAccess.QuickStart
{
    class Program
    {
        static IOpcListener listener;
        static void Main(string[] args)
        {
            OpcConfig config=OpcConfigManager.Configure("General.Index");
            listener=new OpcListener(config);
            listener.OPCDataChanged+=new OpcDataChangedEventHandler(listener_OPCDataChanged);
            listener.Start();
            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();
            listener.Close();
        }

        static void listener_OPCDataChanged(object sender, OpcDataChangedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("Press any key to stop.");
            Hashtable h = e.DataChangedResult;
            foreach (DictionaryEntry de in h)
            {
                Console.WriteLine(de.Key+":"+de.Value);
            }

            
        }
    }
}
