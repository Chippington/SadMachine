using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SadMachine.Activities {
	public class Activity : NetUtils.Net.NetBase {
		public class ScheduledEvent {
			public Action<float> ev;
			public float time;
			public bool repeat;
			internal float next;
		}

		private List<ScheduledEvent> eventList;
		private List<Activity> activities;
		private Activity parent;

		public void initialize() {
			eventList = new List<ScheduledEvent>();
			activities = new List<Activity>();
			onInitialize();
			log("Initialized.");
		}

		/// <summary>
		/// Event driven method called when the activity has been initialized.
		/// </summary>
		public virtual void onInitialize() {
		}

		/// <summary>
		/// Updates the activity.
		/// </summary>
		public virtual void update() {
			performEvents();
			for (int i = activities.Count - 1; i >= 0; i--) {
				activities[i].update();
			}
		}

		/// <summary>
		/// Performs any events that need to be processed.
		/// </summary>
		private void performEvents() {
			for (int i = eventList.Count - 1; i >= 0; i--) {
				if (getTime() > eventList[i].next) {
					eventList[i].ev.Invoke(getTime());
					if (eventList[i].repeat == false) {
						eventList.RemoveAt(i);
					}
					else {
						eventList[i].next = getTime() + eventList[i].time;
					}
				}
			}
		}

		/// <summary>
		/// Adds a subactivity to this activity.
		/// </summary>
		/// <param name="activity"></param>
		public void addActivity(Activity activity) {
			activities.Add(activity);
			activity.parent = this;
			activity.initialize();
			log("Added activity: " + activity.GetType().Name);
		}

		/// <summary>
		/// Removes a activity.
		/// </summary>
		/// <param name="activity"></param>
		public void removeActivity(Activity activity) {
			if (activities.Contains(activity))
				activities.Remove(activity);
			log("Removed activity: " + activity.GetType().Name);
		}

		/// <summary>
		/// Schedules an event to be called every [time] seconds.
		/// </summary>
		/// <param name="ev"></param>
		/// <param name="time"></param>
		/// <param name="repeat"></param>
		public ScheduledEvent addScheduledEvent(Action<float> ev, float time, bool repeat) {
			var ret = new ScheduledEvent() {
				ev = ev,
				time = time,
				repeat = repeat,
				next = getTime() + time
			};

			var curTime = getTime();

			eventList.Add(ret);
			return ret;
		}

		/// <summary>
		/// Schedules an event to be called.
		/// </summary>
		/// <param name="ev"></param>
		/// <returns></returns>
		public ScheduledEvent addScheduledEvent(ScheduledEvent ev) {
			eventList.Add(ev);

			return ev;
		}

		/// <summary>
		/// Removes a scheduled event.
		/// </summary>
		/// <param name="ev"></param>
		public void removeScheduledEvent(ScheduledEvent ev) {
			eventList.Remove(ev);
		}
	}
}
