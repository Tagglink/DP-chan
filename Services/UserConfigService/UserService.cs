using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Discord.WebSocket;
using DP_chan.Services.JsonService;

namespace DP_chan.Services.UserConfigService
{
    class UserService
    {
        private Json json;

        private Dictionary<ulong, User> users;
        private User defaultUser;

        private readonly string userPath;
        private readonly string usersFilename;
        private readonly string usersBackupFilename;

        public UserService(Json json, string userPath)
        {
            this.userPath = userPath;
            this.json = json;
            usersFilename = "users.json";
            usersBackupFilename = "users.json.bak";

            if (!Directory.Exists(userPath)) {
                Directory.CreateDirectory(userPath);
            }

            users = GetSavedUsers();
            defaultUser = new User();
        }

        public string SetUserSetting(ulong userId, string setting, object value)
        {
            User user = users[userId];
            user.settings[setting] = value;

            SaveUsers();

            return @"``" + user.username + "#" + user.tag + ": " + setting + " set to " + value + @"``";
        }

        public bool CheckSettingBool(ulong userId, string setting)
        {
            return (bool)GetSetting(userId, setting);
        }

        public User AddIfNull(SocketUser userData)
        {
            if (UserExists(userData.Id))
            {
                return users[userData.Id];
            }
            else
            {
                User user = CreateUser(userData);
                AddUser(user);
                return user;
            }
        }

        public Dictionary<ulong, User> GetSavedUsers()
        {
            Dictionary<ulong, User> ret = Json.Open<Dictionary<ulong, User>>(userPath + usersFilename);

            if (ret == null)
            {
                ret = new Dictionary<ulong, User>();
            }

            return ret;
        }

        public void SaveUsers(bool backup = false)
        {
            if (backup)
            {
                json.SaveProperly(users, userPath + usersBackupFilename);
            }
            else {
                json.SaveProperly(users, userPath + usersFilename);
            }
        }

        public User GetUser(ulong userId)
        {
            return users[userId];
        }

        public string GetUserInfo(ulong userId)
        {
            User user = GetUser(userId);
            string settingsString = MakeUserSettingsString(user);

            string ret = @"```" + user.username + "#" + user.tag + "'s settings:\n" + settingsString + @"```";

            return ret;
        }

        public List<User> GetUsersBy(Func<User, bool> compareFunc)
        {
            List<User> list = new List<User>();

            foreach (KeyValuePair<ulong, User> element in users)
            {
                if (compareFunc(element.Value))
                {
                    list.Add(element.Value);
                }
            }

            return list;
        }

        private string MakeUserSettingsString(User user)
        {
            UserSettings settings = user.settings;
            string ret = "";

            foreach (KeyValuePair<string, object> pair in settings)
            {
                ret += "  " + pair.Key + ": " + pair.Value + "\n";
            }

            return ret;
        }

        private object GetSetting(ulong userId, string setting)
        {
            return users[userId].settings[setting];
        }

        private void AddUser(User user)
        {
            users.Add(user.userId, user);
            SaveUsers();
        }

        private User CreateUser(SocketUser userData)
        {
            User user = new User(defaultUser);
            user.username = userData.Username;
            user.userId = userData.Id;
            user.tag = userData.Discriminator;

            return user;
        }

        private bool UserExists(ulong userId)
        {
            foreach (KeyValuePair<ulong, User> pair in users)
            {
                if (pair.Key == userId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
