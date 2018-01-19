using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_chan.Main
{
    class SettingsNotFoundException : Exception
    {
        public override string Message {
            get {
                return "Valid settings file not found! Generating...";
            }
        }
    }
}