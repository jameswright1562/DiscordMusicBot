using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot
{

    public class VoiceChannel
    {
        public VoiceChannel(DiscordSocketClient client, IGuild guild, ulong channelId)
        {
            Client = client;
            Channel = GetChannel(guild, channelId).Result;
            AudioClient = Channel.ConnectAsync().Result; 
        }
        public IAudioClient AudioClient { get; private set; }
        public IVoiceChannel Channel { get; private set; }
        public DiscordSocketClient Client { get; private set; }

        public async Task<IVoiceChannel> GetChannel(IGuild guild, ulong channelId)
        {
            return await guild.GetVoiceChannelAsync(channelId, CacheMode.AllowDownload, new RequestOptions
            {
                RetryMode = RetryMode.AlwaysRetry,
                Timeout = 10000
            });
        }

        public async Task SendAsync(string path)
        {
            // Create FFmpeg using the previous example
            if(AudioClient.ConnectionState != ConnectionState.Connected)
                AudioClient = await Channel.ConnectAsync();
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = AudioClient.CreatePCMStream(AudioApplication.Music, 100000))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }

        public async Task StopAsync()
        {
            await AudioClient.StopAsync();
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "C:\\Users\\james\\Downloads\\ffmpeg-2024-01-24-git-00b288da73-full_build\\ffmpeg-2024-01-24-git-00b288da73-full_build\\bin\\ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 0 -f s16le -ar 48000 -q:a 0 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = "C:\\Users\\james\\Downloads\\ffmpeg-2024-01-24-git-00b288da73-full_build\\ffmpeg-2024-01-24-git-00b288da73-full_build\\bin\\"
            });
        }
    }
}
