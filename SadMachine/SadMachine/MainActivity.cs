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

namespace SadMachine
{
	public class MainActivity : ThreadedActivity
	{
		private DiscordClient client;
		public override void onStart() {
			base.onStart();

			client = new DiscordClient();
			client.MessageReceived += client_MessageReceived;
			client.Connect(Config.username, Config.password).Wait();
			Console.WriteLine($"Connected! User: {client.CurrentUser.Name}");

			CommandManager.addCommandHook("test", (cmd) => {
				Console.WriteLine("{0}: Test: {1}", cmd.messageInfo.User.Name, string.Join(",", cmd.args));
			});
		}

		private void client_MessageReceived(object sender, MessageEventArgs e)
		{
			Console.WriteLine(e.Channel.Name + ": " + e.User.Nickname + ": " + e.Message.Text);
			string content = e.Message.Text;

			if (content[0] == '!')
			{
				try {
					string[] split = content.Replace("!", "").Split(' ');
					string[] args = new string[split.Length - 1];
					for (int i = 1; i < split.Length; i++)
						args[i - 1] = split[i];

					CommandManager.invokeCommand(e, split[0], args);
				} catch(Exception ex) {
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}
			}
		}
	}
}
