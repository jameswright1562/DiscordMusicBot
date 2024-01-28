// See https://aka.ms/new-console-template for more information
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.Interactions.Builders;
using Discord.Rest;
using Discord.WebSocket;
using DiscordMusicBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscordBot
{
    class Program
    {
        private readonly IServiceProvider _services;
        public Program()
        {
            _services = CreateProvider();
        }

        static IServiceProvider CreateProvider()
        {
            var config = new DiscordSocketConfig()
            {

            };

            var configuration = BuildConfig().Get<Settings>();
            var collection = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<Settings>(configuration)
                .AddSingleton(x =>
                {
                    return new PlaylistManager(new YoutubeExplode.YoutubeClient(), x.GetRequiredService<DiscordSocketClient>());
                });


                //...
            return collection.BuildServiceProvider();
        }
        static void Main(string[] args)
        {
            new Program().RunAsync(args).GetAwaiter().GetResult();
        }

        async Task RunAsync(string[] args)
        { 

            var client = _services.GetRequiredService<DiscordSocketClient>();
            var settings = _services.GetRequiredService<Settings>();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                UseInteractionSnowflakeDate = true,
                }
            );

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Info
            });



            client.Log += (LogMessage args) => Log(args);
            client.LoginAsync(TokenType.Bot, settings.BotToken).Wait();
            client.StartAsync().Wait();

            
            commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services).Wait();
            while (client.ConnectionState != ConnectionState.Connected) { 
                Task.Delay(1000).Wait();
            }

            Console.WriteLine("Bot is running!");

               
            var playlistManager = new PlaylistManager(new YoutubeExplode.YoutubeClient(), client);
            var commands = new CommandHandler(client, playlistManager, commandService, _services);

            await Task.Delay(Timeout.Infinite);            
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private static IConfigurationRoot BuildConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
