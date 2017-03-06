using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadMachine {
	class Program {
		private static bool exit = false;

		static void Main(string[] args) {
			var cfg = Config.loadConfig();
			if (!cfg) Config.inputConfig();

			var mainActivity = new SadMachine.Activities.MainActivity();
			mainActivity.initialize();

			while(!exit) {
				System.Threading.Thread.Sleep(1);
				mainActivity.update();
			}
		}

		public static void Exit() {
			exit = true;
		}
	}
}
