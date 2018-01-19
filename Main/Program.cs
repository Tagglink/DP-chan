using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using DP_chan.Services.WebFetchService;
using DP_chan.Services.UserConfigService;
using DP_chan.Services.JsonService;
using DP_chan.Services.ImageFetchService;
using DP_chan.CommandModules;

namespace DP_chan.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }

        private CommandService mCommands;
        private DiscordSocketClient mClient;
        private Json json;
        private BotSettings settings;
        private readonly string settingsFilename;

        private Program()
        {
            mClient = new DiscordSocketClient();
            mCommands = new CommandService();
            json = new Json();

            settingsFilename = "settings.json";
            settings = Json.Open<BotSettings>(settingsFilename);
            if (settings == null)
            {
                settings = DefaultSettings();
                json.SaveProperly(settings, settingsFilename);
            }
            
            mClient.Log += Log;
            mCommands.Log += Log;
        }

        private BotSettings DefaultSettings()
        {
            BotSettings s = new BotSettings();
            s.botToken = "<bot_token_here>";
            s.commandPrefix = '-';
            s.dataPath = "data/";

            return s;
        }

        private Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            string token = settings.botToken;

            await InitCommands();

            await mClient.LoginAsync(TokenType.Bot, token);
            await mClient.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider mServices;

        private async Task InitCommands()
        {
            WebFetcher webFetcher = new WebFetcher();
            UserService userService = new UserService(json, settings.dataPath);
            ImageBoardFetcher imageBoardFetcher = new ImageBoardFetcher(json, settings.dataPath + "img/", settings.dataPath);

            mServices = new ServiceCollection()
                .AddSingleton(mClient)
                .AddSingleton(mCommands)
                .AddSingleton(json)
                .AddSingleton(userService)
                .AddSingleton(webFetcher)
                .AddSingleton(imageBoardFetcher)
                .BuildServiceProvider();

            mClient.MessageReceived += HandleCommandAsync;
            
            await mCommands.AddModuleAsync<TestCommands>();
            await mCommands.AddModuleAsync<ImageBoardCommands>();
            await mCommands.AddModuleAsync<UserCommands>();
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;

            if (message.Author.Id == mClient.CurrentUser.Id || message.Author.IsBot) return;

            int pos = 0;

            if (message.HasCharPrefix(settings.commandPrefix, ref pos))
            {
                var context = new SocketCommandContext(mClient, message);

                var result = await mCommands.ExecuteAsync(context, pos, mServices);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}
