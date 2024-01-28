using Discord;
using Discord.WebSocket;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;

namespace DiscordMusicBot
{
    public class Mentions
    {
        public DiscordSocketClient _client { get; set; }
        public VoiceChannel _voiceChannel { get; set; }
        public PlaylistManager _playlistManager { get; set; }
        public Mentions(DiscordSocketClient client, VoiceChannel channel, PlaylistManager playlistManager)
        {
            _client = client;
            _voiceChannel = channel;
            _client.MessageReceived += OnMessageReceived;
            _playlistManager = playlistManager;
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            // Check if the message is from a user and not a bot
            if (message.Author.IsBot)
                return;

            // Check if the message mentions the bot
            if (message.MentionedUsers.Any(user => user.Id == _client.CurrentUser.Id))
            {
                // The bot was mentioned in the message
                if (Regex.IsMatch(message.Content, $@"<@{_client.CurrentUser.Id}> play https?://\S+$"))
                {
                    // Extract the URL from the message
                    var match = Regex.Match(message.Content, $@"<@{_client.CurrentUser.Id}> play (https?://\S+)$");
                    if (match.Success)
                    {
                        string url = match.Groups[1].Value;
                        _playlistManager.PlayYoutubeVideo(url);
                    }
                }
            }
        }


    }
}
