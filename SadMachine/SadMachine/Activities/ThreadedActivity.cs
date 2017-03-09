using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SadMachine.Activities {
	public class ThreadedActivity : Activity {
		protected Thread thread;
		private bool exitFlag;

		public override void onInitialize() {
			base.onInitialize();
			exitFlag = false;

			thread = new Thread(() => {
				onStart();

				while (exitFlag == false) {
					Thread.Sleep(10);
					onStep();
				}

				onTerminate();
			});

			thread.Start();
		}

		public virtual void onStart() { }
		public virtual void onStep() { }
		public virtual void onTerminate() { }

		public void terminate() {
			exitFlag = true;
		}
	}
}
