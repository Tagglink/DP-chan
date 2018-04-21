using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DP_chan.Services.UserConfigService
{
    [JsonDictionary]
    public class UserSettings : Dictionary<string, object>
    { 
        public UserSettings()
        {
            Add("safe", true);
            Add("botadmin", false);
        }

        public UserSettings(string type)
        {
            switch (type)
            {
                case "owner":
                    Add("safe", true);
                    Add("botadmin", true);
                    break;
                case "user":
                    Add("safe", true);
                    Add("botadmin", false);
                    break;
            }
        }

        public bool CheckSettingBool(string setting) {
            if (ContainsKey(setting)){
                return (bool)this[setting];
            } else {
                return false;
            }
        }

        public UserSettings(UserSettings copy) : base(copy)
        {
            
        }
    }
}
