using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DP_chan.Services.UserConfigService
{
    [JsonObject]
    class User
    {
        [JsonProperty]
        public ulong userId;

        [JsonProperty]
        public string username;

        [JsonProperty]
        public string tag;

        [JsonProperty]
        public UserSettings settings;

        public User()
        {
            username = "";
            tag = "";
            userId = 0;
            settings = new UserSettings();
        }

        public User(User copy)
        {
            userId = copy.userId;
            username = copy.username;
            tag = copy.tag;
            settings = new UserSettings(copy.settings);
        }
    }
}
