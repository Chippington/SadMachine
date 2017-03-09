//using DiscordSharp;
//using DiscordSharp.Events;
using Discord.Net;
using Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadMachine
{
	public class MainActivity : Activity
	{

		public override void Start()
		{
			Activity.setMainActivity(this);
			base.Start();

			client = new DiscordClient();
			client.MessageReceived += client_MessageReceived;
			client.Connect(Config.username, Config.password).Wait();
			Console.WriteLine($"Connected! User: {client.CurrentUser.Name}");

			registerCommand("pente", cmdStartPente);
			registerCommand("test", cmdStartTest);
			registerCommand("echo", cmdEcho);
		}

		private void cmdStartTest(Command obj)
		{
			Console.WriteLine("Test!!!");
			addChild(new Activities.TestActivity());
		}

		private void client_MessageReceived(object sender, MessageEventArgs e)
		{
			Console.WriteLine(e.Channel.Name + ": " + e.User.Nickname + ": " + e.Message.Text);
			string content = e.Message.Text;

			if (content[0] == '!')
			{
				handleCommand(e);
			}
		}

		private async void cmdEcho(Command obj)
		{
			await sendMessage(obj.info.Channel, obj.info.Message.Text.Remove(0, 1));
		}

		private async void cmdStartPente(Command obj)
		{
			int size = 0;
			if (int.TryParse(obj.param[1], out size))
			{
				addChild(new Activities.PenteActivity(obj.info.User, size));
				await sendMessage(obj.info.Channel, obj.info.User.Mention + " wants to start a game of pente!" + Environment.NewLine
					+ "Type !joinpente to join the game" + Environment.NewLine
					+ "Type !startpente to start the game" + Environment.NewLine
					+ "Type !setpiece to change your player piece");
			}
		}

		public override void Stop()
		{
			base.Stop();
		}
	}
}
