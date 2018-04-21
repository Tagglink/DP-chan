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
using DP_chan.CommandModules;
using DP_chan.Services.WebFetchService;
using DP_chan.Services.UserConfigService;
using DP_chan.Services.JsonService;
using DP_chan.Services.ImageFetchService;
using DP_chan.Services.FileDatabaseService;

namespace DP_chan.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.MainAsync().GetAwaiter().GetResult();
        }
        private DiscordSocketClient mClient;
        private Json mJson;
        private BotSettings mSettings;
        private readonly string mSettingsFilename;
        private WebFetcher mWebFetcher;
        private UserService mUserService;
        private ImageBoardFetcher mImageBoardFetcher;
        private FileDatabase mFileDatabase;
        private CommandHandler mCommandHandler;

        private Program()
        {
            mClient = new DiscordSocketClient();
            mJson = new Json();

            mSettingsFilename = "settings.json";
            try {
                mSettings = ReadSettings(mSettingsFilename);
            }
            catch (SettingsNotFoundException exc) {
                Console.WriteLine(exc.Message);
                mSettings = DefaultSettings();

                mJson.SaveProperly(mSettings, mSettingsFilename);
            }
            
            mClient.Log += Log;

            InitServices();
            InitCommandHandler();
        }

        private void InitServices() { 
            mWebFetcher = new WebFetcher();
            mUserService = new UserService(mJson, mSettings.dataPath);
            mImageBoardFetcher = new ImageBoardFetcher(mJson, mSettings.dataPath + "img/", mSettings.dataPath);
            mFileDatabase = new FileDatabase(mJson, mWebFetcher, mSettings.dataPath + "files/", mSettings.dataPath);
        }

        private void InitCommandHandler() { 
            mCommandHandler = new CommandHandler(mUserService, mClient, mSettings.commandPrefix);

            IServiceProvider services = new ServiceCollection()
                .AddSingleton(mCommandHandler)
                .AddSingleton(mClient)
                .AddSingleton(mJson)
                .AddSingleton(mUserService)
                .AddSingleton(mWebFetcher)
                .AddSingleton(mImageBoardFetcher)
                .AddSingleton(mFileDatabase)
                .BuildServiceProvider();

            mCommandHandler.Services = services;
            mCommandHandler.Log = Log;
        }

        private BotSettings ReadSettings(string filepath) {
            BotSettings ret;
            ret = Json.Open<BotSettings>(filepath);
            if (ret == null)
            {
                throw new SettingsNotFoundException();
            }

            return ret;
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
            string token = mSettings.botToken;

            await mCommandHandler.InitCommands();

            await mClient.LoginAsync(TokenType.Bot, token);
            await mClient.StartAsync();

            await Task.Delay(-1);
        }
    }
}
