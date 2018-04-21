using System;
using System.Threading.Tasks;
using Discord.Commands;
using DP_chan.CommandModules.CustomAttributes;
using DP_chan.Main;
using DP_chan.Services.FileDatabaseService;

namespace DP_chan.CommandModules
{
    public class FileCommands : ModuleBase<SocketCommandContext>
    {
        public CommandHandler CommandHandler { get; set; }

        public FileDatabase FileDatabase { get; set; }

        [Command("file")]
        private async Task File(string groupArg) {
            string filepath = FileDatabase.GetFile(groupArg);
            await Context.Channel.SendFileAsync(filepath);
        }

        [Command("file")]
        private async Task File(string groupArg, int index) {
            string filepath = FileDatabase.GetFile(groupArg, index - 1);
            await Context.Channel.SendFileAsync(filepath);
        }

        [Command("file")]
        private async Task File(string groupArg, string filename) {
            string filepath = FileDatabase.GetFile(groupArg, filename);
            await Context.Channel.SendFileAsync(filepath);
        }

        [Command("addfile")]
        private async Task AddFile(string groupArg, string uri) {
            string filename = FileDatabase.AddFile(groupArg, uri);
            await Context.Channel.SendMessageAsync(filename + " added to group " + groupArg);
        }

        [Command("removefile")]
        private async Task RemoveFile(string groupArg) {
            FileDatabase.RemoveGroup(groupArg);
            await Context.Channel.SendMessageAsync("File group " + groupArg + " removed.");
        }

        [Command("removefile")]
        private async Task RemoveFile(string groupArg, string filename) {
            FileDatabase.RemoveFile(groupArg, filename);
            await Context.Channel.SendMessageAsync("File " + filename + " removed from group " + groupArg + ".");
        }

        [Command("listfile")]
        private async Task ListFile() {
            string[] groups = FileDatabase.GetFileGroups();
            string m = @"```";

            foreach (string group in groups) {
                m += "\n" + group;
            }

            m += @"```";

            await Context.Channel.SendMessageAsync(m);
        }

        [Command("listfile")]
        private async Task ListFile(string group) {
            string[] files = FileDatabase.GetFiles(group);
            string m = @"```";

            for (int i = 0; i < files.Length; i++) {
                m += "\n" + (i + 1) + ": " + files[i];
            }

            m += @"```";

            await Context.Channel.SendMessageAsync(m);
        }
    }
}