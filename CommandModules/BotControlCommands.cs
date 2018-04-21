using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using DP_chan.CommandModules.CustomAttributes;
using DP_chan.Main;
using DP_chan.Services.UserConfigService;

namespace DP_chan.CommandModules
{
    public class BotControlCommands : ModuleBase<SocketCommandContext>
    {
        public CommandHandler CommandHandler { get; set; }

        public UserService UserService { get; set; }

        [RequirePermission("botadmin")]
        [Command("pause")]
        private async Task Pause() {
            CommandHandler.Paused = true;
            await Context.Channel.SendMessageAsync("Pausing command execution.");
        }

        [Unpausable]
        [RequirePermission("botadmin")]
        [Command("unpause")]
        private async Task Unpause() {
            if (CommandHandler.Paused) {
                CommandHandler.Paused = false;
                await Context.Channel.SendMessageAsync("Resuming command execution.");
            }
            else {
                await Context.Channel.SendMessageAsync("Already executing commands.");
            }
        }

        [Command("memory")]
        private async Task Memory() {
            DriveInfo[] drives = DriveInfo.GetDrives();
            string message = "Drive was not ready";
            DriveInfo root;
            if (drives.Length > 0){
                root = drives[0];
                if (root.IsReady) {
                    long megabytes = root.AvailableFreeSpace / 0x100000;
                    message = "I have " + megabytes + " MB remaining";
                }
            }

            await Context.Channel.SendMessageAsync(message);
        }
    }
}