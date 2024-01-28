using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;

namespace DiscordMusicBot
{
    public class CommandHandler : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient _client { get; set; }
        public PlaylistManager _playlistManager { get; set; }
        public CommandService _commands { get; set; }
        public IServiceProvider Service { get; set; }
        public CommandHandler(DiscordSocketClient client, PlaylistManager playlistManager, CommandService commands, IServiceProvider service)
        {
            _client = client;
            _client.MessageReceived += HandleCommand;
            _playlistManager = playlistManager;
            _commands = commands;
            Service = service;
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            var res = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: Service);

            if(res.IsSuccess)
            {
                Console.WriteLine("Command executed successfully");
            }
            else
            {
                Console.WriteLine(res.ErrorReason);
            }
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string url)
        {
            // _playlistManager.PlayYoutubeVideo(url);
            var message = Context.Message;
            var author = Context.Message.Author as SocketGuildUser;
            var guild = author.Guild;
            var vc = guild.VoiceChannels.First(x=>x.ConnectedUsers.Select(x=>x.Id).Contains(message.Author.Id));
        
            var _voiceChannel = new VoiceChannel(_client, vc.Guild, vc.Id);
            _playlistManager.PlayYoutubeVideo(url, _voiceChannel);
            return;
        }

        public async Task OnMessageReceived(SocketMessage message)
        {
            // Check if the message is from a user and not a bot
            if (message.Author.IsBot)
                return;


        }

        public async Task PlaySong()
        {

        }


    }
}
