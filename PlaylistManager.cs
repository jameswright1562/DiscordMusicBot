using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace DiscordMusicBot
{
    public class PlaylistManager
    {
        public List<string> Playlist { get; set; }
        public YoutubeClient YoutubeClient { get; set; }
        public VoiceChannel _voiceChannel { get; set; }
        public DiscordSocketClient _client { get; set; }
        public PlaylistManager(YoutubeClient youtubeClient, VoiceChannel voiceChannel, DiscordSocketClient client) 
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ClearFiles);
            Playlist = new List<string>();
            YoutubeClient = youtubeClient;
            _voiceChannel = voiceChannel;
            _client = client;
        }

        public void Add(string url)
        {
            Playlist.Add(url);
        }

        public void ClearFiles(object sender, EventArgs e)
        {
            Playlist.ForEach(file => System.IO.File.Delete(Path.Combine(Environment.CurrentDirectory, file)));
        }

        public void PlayYoutubeVideo(string url)
        {
            var video = YoutubeClient.Videos.GetAsync(url).Result;
            var streamManifest = YoutubeClient.Videos.Streams.GetManifestAsync(video.Id).Result;
            var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            var videoUrl = $"./{video.Id}.mp3";
            if(!System.IO.File.Exists(Path.Combine(Environment.CurrentDirectory, videoUrl)))
                YoutubeClient.Videos.Streams.DownloadAsync(audioStreamInfo, videoUrl).AsTask().Wait();
                        
             _voiceChannel.StopAsync().Wait();
             _voiceChannel.SendAsync(Path.Combine(Environment.CurrentDirectory, videoUrl));

             _client.SetGameAsync(video.Title).Wait();
            Add(url);
        }
    }
}
