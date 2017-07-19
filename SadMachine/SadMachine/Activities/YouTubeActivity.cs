using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadMachine.Activities;
using SadMachine.Commands;
using Discord.WebSocket;
using Discord.Net.Providers.WS4Net;
using System.Diagnostics;
using Discord.Audio;
using VideoLibrary;
using MediaToolkit.Model;
using MediaToolkit;
using Discord;

namespace SadMachine.Activities {
	class YouTubeActivity : Activity, IMusicActivity {
		private IAudioClient audio;
		public bool isPlaying = false;
		private Process ffmpeg;
		AudioOutStream stream;

		public YouTubeActivity() {
		}

		public override void onInitialize() {
			base.onInitialize();
		}

		public void playFromUri(string uri, IAudioClient audio) {
			isPlaying = true;

			YouTube youtube = YouTube.Default;
			var vid = youtube.GetVideo(uri);

			string vidName = getUniqueName(vid, uri);
			string mp3Name = vidName + ".mp3";

			if (System.IO.File.Exists(mp3Name) == false) {
				var bytes = vid.GetBytes();
				System.IO.File.WriteAllBytes(vidName + vid.FileExtension, bytes);

				var inputFile = new MediaFile { Filename = vidName + vid.FileExtension };
				var outputFile = new MediaFile { Filename = mp3Name };

				using (var engine = new Engine()) {
					engine.GetMetadata(inputFile);
					engine.Convert(inputFile, outputFile);
				}

				System.IO.File.Delete(inputFile.Filename);
			}

			var t = SendAsync(audio, mp3Name);
		}

		public bool getIsPlaying() {
			return isPlaying;
		}

		private string getUniqueName(YouTubeVideo video, string uri) {
			string prefix = "";
			if(uri.Contains("youtu.be")) {
				int start = uri.Length - 1;
				for(int i = start; i >= 0; i--, start--) {
					if (uri[i] == '/')
						break;
				}

				prefix = uri.Substring(start, uri.Length - start).Replace("/", "");
			} else if(uri.Contains("watch?v=")) {

				int start = uri.IndexOf("?v=") + 3;
				int end = uri.IndexOf("&", start);

				if (end < 0)
					end = uri.Length - 1;

				prefix = uri.Substring(start, end - start);
			} else {
				prefix = "UNKNOWN";
			}

			return prefix + "-" + video.FullName.Replace(" ", "").Replace(video.FileExtension, "");
		}

		private Process CreateStream(string path) {
			var ffmpeg = new ProcessStartInfo {
				FileName = "ffmpeg",
				Arguments = $"-i {path} -ac 2 -af \"volume=0.25\" -f s16le -v \"error\" -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true,
			};
			return Process.Start(ffmpeg);
		}

		private async Task SendAsync(IAudioClient client, string path) {
			isPlaying = true;
			// Create FFmpeg using the previous example
			await Task.Delay(1000);

			ffmpeg = CreateStream(path);
			var output = ffmpeg.StandardOutput.BaseStream;
			stream = client.CreatePCMStream(AudioApplication.Mixed);
			await output.CopyToAsync(stream);
			await stream.FlushAsync();
			isPlaying = false;
		}

		public void prepareFromUri(string uri) {
			YouTube youtube = YouTube.Default;
			var vid = youtube.GetVideo(uri);

			string vidName = getUniqueName(vid, uri);
			string mp3Name = vidName + ".mp3";

			if (System.IO.File.Exists(mp3Name) == false) {
				var bytes = vid.GetBytes();
				System.IO.File.WriteAllBytes(vidName + vid.FileExtension, bytes);

				var inputFile = new MediaFile { Filename = vidName + vid.FileExtension };
				var outputFile = new MediaFile { Filename = mp3Name };

				using (var engine = new Engine()) {
					engine.GetMetadata(inputFile);
					engine.Convert(inputFile, outputFile);
				}

				System.IO.File.Delete(inputFile.Filename);
			}
		}

		public void cancelCurrentSong() {
			if(ffmpeg != null) {
				//ffmpeg.Close();
				ffmpeg.Kill();
				stream.Clear();
				ffmpeg = null;
				isPlaying = false;
			}
		}

		public string getSongName(string uri) {
			YouTube youtube = YouTube.Default;
			var vid = youtube.GetVideo(uri);
			return vid.FullName;
		}
	}
}
