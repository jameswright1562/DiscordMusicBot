// See https://aka.ms/new-console-template for more information
using Discord;
using Discord.Audio;
using Discord.Rest;
using Discord.WebSocket;
using DiscordMusicBot;

namespace DiscordBot
{
    class Program
    {
        private static DiscordSocketClient _client;
        static void Main(string[] args)
        {
        /*    Console.WriteLine("What is your bot token?");
            
            string token = Console.ReadLine();

            Console.WriteLine("What is your discord developer id?");

            ulong guildId = Convert.ToUInt64(Console.ReadLine());

            */

            _client = new DiscordSocketClient();

            _client.Log += (LogMessage args) => Log(args);
            _client.LoginAsync(TokenType.Bot, "MTE5OTgyNDM1OTA2MjI2MTgxMg.GT5pe2.iuwha4zDGSxW-iCP0OrlafvXba-GyjAXIooDGs").Wait();
            _client.StartAsync().Wait();

            while (_client.ConnectionState != ConnectionState.Connected) { 
                Task.Delay(1000).Wait();
            }

            Console.WriteLine("Bot is running!");

                
            var user = _client.GetGuild(1201129685443153920);

            var vc = new VoiceChannel(_client, user.CurrentUser, "1201129685443153924");

            var playlistManager = new PlaylistManager(new YoutubeExplode.YoutubeClient(), vc, _client);
            var mentions = new Mentions(_client, vc, playlistManager);

            
            while(true)
            {
                Task.Delay(1000).Wait();
            }        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
    }
}
