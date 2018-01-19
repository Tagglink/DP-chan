using System;
using System.Threading.Tasks;
using Discord.Commands;
using DP_chan.CommandModules.CustomAttributes;
using DP_chan.Main;

namespace DP_chan.CommandModules
{
    public class BotControlCommands : ModuleBase<SocketCommandContext>
    {
        public CommandHandler CommandHandler { get; set; }

        [Command("pause")]
        private async Task Pause() {
            if (CommandHandler.Paused) {
                await Context.Channel.SendMessageAsync("Already paused.");
            }
            else {
                CommandHandler.Paused = true;
                await Context.Channel.SendMessageAsync("Pausing command execution.");
            }
        }

        [Unpausable]
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
    }
}