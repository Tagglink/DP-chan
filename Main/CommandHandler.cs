using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DP_chan.CommandModules;
using DP_chan.CommandModules.CustomAttributes;
using DP_chan.Services.ImageFetchService;
using DP_chan.Services.UserConfigService;
using DP_chan.Services.WebFetchService;
using Microsoft.Extensions.DependencyInjection;


namespace DP_chan.Main {
    public class CommandHandler {
        
        public IServiceProvider Services {
            set {
                mServices = value;
            }
        }

        public Func<LogMessage, Task> Log {
            set {
                mCommands.Log += value;
            }
        }
        public bool Paused { get; set; }

        private CommandService mCommands;

        private IServiceProvider mServices;

        private DiscordSocketClient mClient;
        private char mCommandPrefix;

        public CommandHandler(DiscordSocketClient client, char commandPrefix) { 
            Paused = false;
            mClient = client;
            mCommandPrefix = commandPrefix;
            mCommands = new CommandService();
        }

        public async Task InitCommands()
        {
            mClient.MessageReceived += HandleCommandAsync;
            
            await mCommands.AddModuleAsync<TestCommands>();
            await mCommands.AddModuleAsync<ImageBoardCommands>();
            await mCommands.AddModuleAsync<UserCommands>();
            await mCommands.AddModuleAsync<BotControlCommands>();
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;

            if (message.Author.Id == mClient.CurrentUser.Id || message.Author.IsBot) return;

            int pos = 0;

            if (message.HasCharPrefix(mCommandPrefix, ref pos))
            {
                if (Paused && !CommandIsUnpausable(message.Content)) return;

                var context = new SocketCommandContext(mClient, message);

                var result = await mCommands.ExecuteAsync(context, pos, mServices);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        private bool CommandIsUnpausable(string message) {
            CommandInfo command = GetCommand(message);
            if (command == null) return false;

            return command.Attributes.Any((attr) => attr is UnpausableAttribute);
        }

        private CommandInfo GetCommand(string message) { 
            string commandName = GetCommandString(message);
            foreach (ModuleInfo module in mCommands.Modules) {
            foreach (CommandInfo command in module.Commands) {
                if (commandName == command.Name)
                    return command;
            }
            }

            return null;
        }

        private string GetCommandString(string message) {
            message = message.Remove(0, 1);

            for (int i = 0; i < message.Length; i++) {
                char c = message[i];
                if (c == ' ') {
                    message = message.Remove(i);
                    break;
                }
            }
            
            return message;
        }
    }
}