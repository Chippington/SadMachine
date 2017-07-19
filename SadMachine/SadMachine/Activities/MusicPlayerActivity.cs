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
	class MusicPlayerActivity : Activity {
		Queue<string> uriQueue;
		DiscordSocketClient client {
			get {
				return MainClient.client;
			}
		}
		YouTubeActivity youtube;
		IMusicActivity current;
		IAudioClient audio;
		bool firstJoin;

		public MusicPlayerActivity() {
		}

		public override void onInitialize() {
			base.onInitialize();
			firstJoin = true;
			uriQueue = new Queue<string>();

			youtube = new YouTubeActivity();
			addActivity(youtube);


			CommandManager.addCommandHook("q", (cmd) => {
				log("Queue request from " + cmd.messageInfo.Author.Username);

				if(cmd.args.Length == 0) {
					return;
				}

				handleMusicRequest(cmd);
			});

			CommandManager.addCommandHook("skip", (cmd) => {
				if (current == null)
					return;

				current.cancelCurrentSong();
			});

			addScheduledEvent((val) => update(val), 0.3f, true);
		}

		private async void handleMusicRequest(Command cmd) {
			try {
				var channel = (cmd.messageInfo.Author as IGuildUser)?.VoiceChannel;
				var user = channel.GetUserAsync(client.CurrentUser.Id).GetAwaiter().GetResult();

				if (audio == null)
					audio = await channel.ConnectAsync();

				if (user != null && firstJoin || audio.ConnectionState == ConnectionState.Disconnected) {
					audio.Dispose();
					audio = await channel.ConnectAsync();
				}

				//if (user == null || firstJoin)
				firstJoin = false;
				var player = getPlayerFromUri(cmd.args[0]);

				if (current != null && current.getIsPlaying()) {

					if (player == null)
						return;

					player.prepareFromUri(cmd.args[0]);

					log("Enqueuing: " + cmd.args[0]);
					await cmd.messageInfo.Channel.SendMessageAsync("Enqueuing: " + player.getSongName(cmd.args[0]));
					uriQueue.Enqueue(cmd.args[0]);
					return;
				} else {
					await cmd.messageInfo.Channel.SendMessageAsync("Now playing: " + player.getSongName(cmd.args[0]));
				}

				uriQueue.Enqueue(cmd.args[0]);
			} catch (Exception ex) {
				log(ex.Message);
				log(ex.StackTrace);
			}
		}

		private void update(double time) {
			if (uriQueue.Count == 0)
				return;

			if (current != null && current.getIsPlaying())
				return;

			string next = uriQueue.Dequeue();
			var player = getPlayerFromUri(next);

			if (player == null)
				return;

			player.playFromUri(next, audio);
			current = player;
		}

		private IMusicActivity getPlayerFromUri(string uri) {
			if (uri.Contains("youtu")) {
				log("Youtube URI discovered: " + uri);
				return youtube;
			}

			return null;
		}
	}
}
