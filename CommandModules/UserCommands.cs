using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DP_chan.Services.UserConfigService;
using Discord.WebSocket;
using DP_chan.Extensions;

namespace DP_chan.CommandModules
{
    class UserCommands : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient Client
        {
            set
            {
                client = value;
            }
        }

        public UserService UserService
        {
            set
            {
                users = value;
            }
        }

        private UserService users;
        private DiscordSocketClient client;

        [Command("myset")]
        private async Task MySet()
        {
            users.AddIfNull(Context.User);
            string userInfo = users.GetUserInfo(Context.User.Id);
            await Context.Channel.SendMessageAsync(userInfo);
        }

        [Command("myset")]
        private async Task MySet(string setting, bool value)
        {
            users.AddIfNull(Context.User);
            string info = users.SetUserSetting(Context.User.Id, setting, value);
            await Context.Channel.SendMessageAsync(info);
        }

        [Command("uset")]
        private async Task USet(ulong userId)
        {
            SocketUser user = client.GetUser(userId);
            users.AddIfNull(user);
            string userInfo = users.GetUserInfo(userId);
            await Context.Channel.SendMessageAsync(userInfo);
        }

        [Command("uset")]
        private async Task USet(ulong userId, string setting, bool value)
        {
            SocketUser userData = client.GetUser(userId);
            users.AddIfNull(userData);
            string info = users.SetUserSetting(userId, setting, value);
            await Context.Channel.SendMessageAsync(info);
        }

        [Command("uset")]
        private async Task USet(string mention)
        {
            ulong userId = StringExtensions.GetDiscordUserIdFromMention(mention);
            await USet(userId);
        }

        [Command("uset")]
        private async Task USet(string mention, string setting, bool value)
        {
            ulong userId = StringExtensions.GetDiscordUserIdFromMention(mention);
            await USet(userId, setting, value);
        }

        [Command("save-backup")]
        private async Task Save()
        {
            users.SaveUsers(true);
            await Context.Channel.SendMessageAsync("User list backup saved.");
        }
    }
}
