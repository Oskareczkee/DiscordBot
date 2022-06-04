using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using Newtonsoft.Json;

using DiscordBot.Commands;
using DSharpPlus.Entities;
using Bot.Commands;
using Bot.Commands.Helpers;
using Bot.Commands.ProfileManagment;

namespace DiscordBot
{
    public class Bot
    {
        //this class grants easy access to bot config properties
        public class BotConfig
        {
            public string prefix { get; set; }
            public string mamonPhotoURL { get; set; }
            public ulong IDPawla { get; set; }
            public string MemeFolderRoot { get; set; }
        }

        public DiscordClient Client { get; private set; }// private set means we can set this Client only in this class
        public CommandsNextExtension Commands { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public DiscordActivity Activity { get; private set; }

        //use it only when necessary or in attributes, othwerwise use constructor injection
        public static  IServiceProvider Services { get; private set; }

        public static BotConfig Configuration { get; private set; }

        public Bot(IServiceProvider services)
        {

            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var Config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            };

            Configuration = new BotConfig
            {
                prefix = configJson.CommandPrefix,
                mamonPhotoURL = configJson.mamonPhotoURL,
                IDPawla = configJson.IDPawla,
                MemeFolderRoot = configJson.MemeFolderRoot
            };

            Activity = new DiscordActivity
            {
                Name = "Polska rodzina - cwel i wazelina!",
                ActivityType = ActivityType.Streaming,
                StreamUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            };

            Client = new DiscordClient(Config);
            Client.Ready += OnClientReady;
            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var CommandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.CommandPrefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                Services = services
            };

            Services = services;

            Commands = Client.UseCommandsNext(CommandsConfig);
            //command registration
            registerCommands();

            Client.ConnectAsync(Activity);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        //put all commands registers here
        private Task registerCommands()
        {
            //basic commands
            Commands.RegisterCommands<TestCommands>();
            Commands.RegisterCommands<ManageCommands>();
            Commands.RegisterCommands<BasicCommands>();

            //shop commands
            Commands.RegisterCommands<DBShopCommands>();

            //profile commands
            Commands.RegisterCommands<DBProfileCommands>();
#if DATABASE_CLEAR
            Commands.RegisterCommands<ClearDatabase>();
#endif

            return Task.CompletedTask;
        }


    }
}
