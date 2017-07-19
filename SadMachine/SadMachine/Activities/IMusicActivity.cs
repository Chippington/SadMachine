using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadMachine.Activities {
	public interface IMusicActivity {
		void prepareFromUri(string uri);
		void playFromUri(string uri, IAudioClient audio);
		bool getIsPlaying();
		void cancelCurrentSong();
		string getSongName(string uri);
	}
}
