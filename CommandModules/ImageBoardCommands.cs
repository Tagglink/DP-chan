using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DP_chan.Services.ImageFetchService;
using DP_chan.Services.UserConfigService;

namespace DP_chan.CommandModules
{
    class ImageBoardCommands : ModuleBase<SocketCommandContext>
    {
        public ImageBoardFetcher ImageBoardFetcher
        {
            set
            {
                imageBoardFetcher = value;
            }
        }

        public UserService UserService
        {
            set
            {
                users = value;
            }
        }

        private ImageBoardFetcher imageBoardFetcher;
        private UserService users;

        [Command("image")]
        private async Task Image(string board, string tagStr, int page = 1, int index = 0)
        {
            ulong userId = Context.User.Id;
            users.AddIfNull(Context.User);
            bool safe = users.CheckSettingBool(userId, "safe");

            string[] tags = tagStr.Split(' ');
            string filepath = imageBoardFetcher.GetImage(board, tags, page, index - 1, safe);

            await Context.Channel.SendFileAsync(filepath);
        }

        [Command("kona")]
        private async Task Kona(params string[] tagStr)
        {
            string tags = string.Join(" ", tagStr);
            await Image("konachan", tags);
        }

        [Command("yan")]
        private async Task Yan(params string[] tagStr)
        {
            string tags = string.Join(" ", tagStr);
            await Image("yandere", tags);
        }

        [Command("dan")]
        private async Task Dan(params string[] tagStr)
        {
            string tags = string.Join(" ", tagStr);
            await Image("danbooru", tags);
        }
    }
}
