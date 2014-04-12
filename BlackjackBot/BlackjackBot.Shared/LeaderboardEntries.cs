using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackBot.Shared
{
	/// <summary>
	/// Entry for the leaderboard display
	/// </summary>
	public class LeaderboardEntries
	{
		/// <summary>
		/// Name of bot
		/// </summary>
		public string BotName { get; set; }

		/// <summary>
		/// Balance of bot
		/// </summary>
		public decimal Balance { get; set; }
	}
}
