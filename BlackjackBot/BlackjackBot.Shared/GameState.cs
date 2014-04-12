using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace BlackjackBot.Shared
{
    /// <summary>
    /// GameState represents the state of the current game
    /// </summary>
	public class GameState
	{
        /// <summary>
        /// Server time when a game has started. 
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Server time when a game has ended.
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// a GUID unique identifier for a game series.
        /// </summary>
		public string GameSeriesId { get; set; }

        /// <summary>
        /// A counter reprsenting the current game being played. 
        /// </summary>
		public int GameNumber { get; set; }

        /// <summary>
        /// The total # of games that will be played. 
        /// </summary>
        public int TotalGames { get; set; }

        /// <summary>
        /// A helper method that just points to your player. 
        /// </summary>
		public PlayerState Me { get; set; }

        /// <summary>
        /// A list of all players in a game series.
        /// </summary>
		public List<PlayerState> AllPlayers { get; set; }

        /// <summary>
        /// Minimum bet amount
        /// </summary>
        public const decimal MinimumBet = 5;

        /// <summary>
        /// Maximum bet amount
        /// </summary>
        public const decimal MaximumBet = 5000;

        /// <summary>
        /// Name/Value pairs for GameState data properties except players
        /// </summary>
        /// <returns>A string representation of the GameState</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(); 
            List<string> exclude = new List<string> { "AllPlayers"};
            sb.AppendLine(TypeHelper.GetTypes(this, exclude)); 

            return sb.ToString(); 
        }

        /// <summary>
        /// Displays Name, balance, wins,losses, pushes, sorted by top balance
        /// </summary>
        /// <returns>string representing each player. </returns>
        public string DisplayResults()
        {
            if (AllPlayers == null)
            {
                return null; 
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name, Balance, Wins, Losses, Pushes");           

            foreach (PlayerState p in AllPlayers.OrderByDescending(x => x.Balance))
            {
                sb.AppendFormat("{0},{1},{2},{3},{4}", p.Name, (int)p.Balance, p.Wins, p.Losses, p.Pushes); 
            }

            return sb.ToString();                     
        }
	}
}
