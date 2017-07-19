using Discord;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadMachine {
	public static class MainClient {
		public static Func<SocketMessage, Task> onMessageReceived;

		private static DiscordSocketClient _client;
		public static DiscordSocketClient client {
			get {
				if(_client == null) {
					string token = "";

					_client = new DiscordSocketClient(new DiscordSocketConfig() {
						WebSocketProvider = WS4NetProvider.Instance,
					});
					_client.Log += MainClient.Log;
					_client.MessageReceived += client_MessageReceived;
					_client.LoginAsync(TokenType.Bot, token);
					_client.StartAsync();
				}
				
				return _client;
			}
		}

		public static async Task Refresh() {
			_client.Dispose();
			_client = null;

			string token = "";

			_client = new DiscordSocketClient(new DiscordSocketConfig() {
				WebSocketProvider = WS4NetProvider.Instance,
			});
			_client.Log += MainClient.Log;
			_client.MessageReceived += client_MessageReceived;

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			while (_client.ConnectionState == ConnectionState.Connecting) ;
		}

		private static Task client_MessageReceived(SocketMessage arg) {
			if (onMessageReceived != null)
				return onMessageReceived(arg);

			return Task.CompletedTask;
		}

		private static Task Log(LogMessage msg) {
			var m = msg;
			Console.WriteLine(m.ToString());
			return Task.CompletedTask;
		}
	}
}
