using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SadMachine {
	public class Config {
		private const string CFGFILENAME = "sad.cfg";
		private static string _username;
		private static string _password;

		public static string username {
			get { return _username; }
			private set { _username = value; }
		}

		public static string password {
			get { return _password; }
			private set { _password = value; }
		}

		public static void saveConfig() {
			if (File.Exists(CFGFILENAME))
				File.Delete(CFGFILENAME);

			File.WriteAllLines(CFGFILENAME, new string[] {
				_username,
				_password
			});
		}

		public static bool loadConfig() {
			if (File.Exists(CFGFILENAME) == false)
				return false;

			try {
				var data = File.ReadAllLines(CFGFILENAME);
				_username = data[0].Trim();
				_password = data[1].Trim();
			} catch (Exception e) {
				Console.WriteLine(e.Message, e.StackTrace);
				return false;
			}

			return true;
		}

		public static void inputConfig() {
			Console.WriteLine("Input credentials:");
			_username = Console.ReadLine().Trim();
			_password = Console.ReadLine().Trim();

			Console.WriteLine("Config saved as: " + CFGFILENAME);
			saveConfig();
		}
	}
}
