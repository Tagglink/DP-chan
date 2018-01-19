using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DP_chan.Services.UserConfigService;
using DP_chan.Services.ImageFetchService;
using DP_chan.Services.WebFetchService;

namespace DP_chan.CommandModules
{
    class TestCommands : ModuleBase<SocketCommandContext>
    {
        public ImageBoardFetcher ImageBoardFetcher
        {
            set
            {
                imageBoardFetcher = value;
            }
        }

        public WebFetcher WebFetcher
        {
            set
            {
                webFetcher = value;
            }
        }

        private ImageBoardFetcher imageBoardFetcher;
        private WebFetcher webFetcher;

        [Command("ping")]
        [Summary("Sends back a pong.")]
        private async Task Ping()
        {
            await Context.Channel.SendMessageAsync("You can't tell me what to do.");
        }

        [Command("read")]
        private async Task Read(string url)
        {
            string content = webFetcher.DownloadDocumentString(url);
            await Context.Channel.SendMessageAsync(content.Remove(256, content.Length - 256));
        }

        [Command("mention")]
        private async Task Mention()
        {
            await Context.Channel.SendMessageAsync("Hello, " + Context.User.Mention + " !");
        }
    }
}
