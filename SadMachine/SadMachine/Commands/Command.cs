using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Net;
using Discord;
using Discord.WebSocket;

namespace SadMachine.Commands {
	public class Command {
		public SocketMessage messageInfo;
		public string[] args;
	}
}
