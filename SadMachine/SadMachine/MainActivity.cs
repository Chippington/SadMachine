//using DiscordSharp;
//using DiscordSharp.Events;
using Discord.Net;
using Discord;

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

namespace SadMachine
{
	public class MainActivity : Activity
	{
		public override void onInitialize() {
			base.onInitialize();

			MainClient.onMessageReceived = client_MessageReceived;

			MusicPlayerActivity musicPlayer = new MusicPlayerActivity();
			addActivity(musicPlayer);
		}

		private Task client_MessageReceived(SocketMessage arg) {
			Console.WriteLine(arg.Channel.Name + ": " + arg.Author.Username + ": " + arg.Content);
			string content = arg.Content;

			if (content[0] == '!') {
				try {
					string[] split = content.Replace("!", "").Split(' ');
					string[] args = new string[split.Length - 1];
					for (int i = 1; i < split.Length; i++)
						args[i - 1] = split[i];

					CommandManager.invokeCommand(arg, split[0], args);
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}
			}

			return Task.CompletedTask;
		}

		private Task Log(LogMessage msg) {
			var m = msg;
			Console.WriteLine(m.ToString());
			return Task.CompletedTask;
		}
	}
}
