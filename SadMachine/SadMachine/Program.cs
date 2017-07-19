using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SadMachine
{
	class Program
	{
		static void Main(string[] args)
		{
			MainActivity activity = new MainActivity();
            if (Config.loadConfig() == false)
                Config.inputConfig();

			var cl = MainClient.client;
			activity.initialize();

			while (true) {
				Thread.Sleep(10);
				activity.update();
			}
		}
	}
}
