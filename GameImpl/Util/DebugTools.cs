using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Base;

namespace CWLEngine.GameImpl.Util
{
    public class DebugTools
    {
        public static string Msg { get; set; } = "";

        public static void AddLine(string msg)
        {
            Msg += "\n" + msg;
        }

        public static void Clear()
        {
            Msg = "";
        }
    }
}
