using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace SadMachine.Commands {
	public static class CommandManager {
		private static Dictionary<string, List<Action<Command>>> callbackMap = new Dictionary<string, List<Action<Command>>>();

		public static void addCommandHook(string command, Action<Command> callback) {
			if (callbackMap.ContainsKey(command) == false)
				callbackMap.Add(command, new List<Action<Command>>());

			callbackMap[command].Add(callback);
		}

		public static void removeCommandHook(string command, Action<Command> callback) {
			if (callbackMap.ContainsKey(command))
				callbackMap[command].Remove(callback);
		}

		public static void invokeCommand(User sender, string cmdName, string[] args) {
			if(callbackMap.ContainsKey(cmdName)) {
				Command cmd = new Command();
				cmd.client = sender;
				cmd.args = args;

				foreach (Action<Command> a in callbackMap[cmdName])
					a?.Invoke(cmd);
			}
		}
	}
}
