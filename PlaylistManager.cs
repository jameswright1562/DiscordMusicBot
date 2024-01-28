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
    public class Song
    {
        public Task Task { get; set; }
        public string Url { get; set; }
    }
    public class PlaylistManager
    {
        public List<string> Playlist { get; set; }
        public YoutubeClient YoutubeClient { get; set; }
        public DiscordSocketClient _client { get; set; }
        public PlaylistManager(YoutubeClient youtubeClient, DiscordSocketClient client) 
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ClearFiles);
            Playlist = new List<string>();
            YoutubeClient = youtubeClient;
            _client = client;
            var timer = new System.Timers.Timer(1000);
            timer.
        }

        public void Add(string url)
        {
            Playlist.Add(url);
        }

        public void Remove(string url)
        {
            Playlist.Remove(url);
        }

        public void ClearFiles(object sender, EventArgs e)
        {
            Playlist.ForEach(file => System.IO.File.Delete(Path.Combine(Environment.CurrentDirectory, file)));
        }

        public void PlayYoutubeVideo(string url, VoiceChannel channel)
        {
            var video = YoutubeClient.Videos.GetAsync(url).Result;
            var streamManifest = YoutubeClient.Videos.Streams.GetManifestAsync(video.Id).Result;
            var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            var videoUrl = $"./{video.Id}.mp3";
            if(!System.IO.File.Exists(Path.Combine(Environment.CurrentDirectory, videoUrl)))
                YoutubeClient.Videos.Streams.DownloadAsync(audioStreamInfo, videoUrl).AsTask().Wait();
                        
             channel.StopAsync().Wait();
            
             Add(url);

             _client.SetGameAsync(video.Title).Wait();
             channel.SendAsync(Path.Combine(Environment.CurrentDirectory, videoUrl)).ContinueWith(x=>
             {
                 Remove(url);
                 if(Playlist.Count > 0)
                 {
                     PlayYoutubeVideo(Playlist[0], channel);
                 }
             });
        }
    }
}
