using System;
namespace BlackjackBot.Shared
{
    /// <summary>
    /// Represents the state of a player.
    /// </summary>
	public class PlayerState 
	{
        /// <summary>
        /// Represents a unique identifier for the player. This is not used on the client
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// a GUID representing a unique GameSeries
        /// </summary>
        public string GameSeriesId { get; set; }

        /// <summary>
        /// A true/false indicating if the player can double down. 
        /// </summary>
        public bool CanDoubleDown { get; set; }

        /// <summary>
        /// True/false indicating if the player paid to see the dealers card.
        /// </summary>
        public bool PaidToSeeDealersCard { get; set; }
        
        /// <summary>
        /// True/flag if the players turn completed
        /// </summary>
        public bool TurnCompleted { get; set; }
        
        /// <summary>
        /// A unique value that you must get from registering your bot
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// The unique key given when registering your bot
		/// </summary>
		public string Key { get; set; }

        /// <summary>
        /// How much a player bet. 
        /// </summary>
        public decimal Bet { get; set; }
        
        /// <summary>
        /// Number of times a player has timed out. After three, you are considered out of the game.
        /// </summary>
        public int Timeouts { get; set; }

        /// <summary>
        /// A True/False flag if a valid PlayerMove was not received in two seconds. 
        /// </summary>
        public bool MoveTimeout { get; set; }
               
        /// <summary>
        /// The players cumulative balance across all games
        /// </summary>
        public decimal Balance { get; set; }
        
        /// <summary>
        /// The end result for a game
        /// </summary>
        public EndResult Result { get; set; }
        
        /// <summary>
        /// The players cards
        /// </summary>
        public Hand Hand { get; set; }
        
        /// <summary>
        /// True/false if the player is a dealer.
        /// </summary>
        public bool IsDealer { get; set; }

        /// <summary>
        /// Total # of wins in a GameSeries.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Total # of losses in a GameSeries.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Total # of pushes (ties) in a GameSeries.
        /// </summary>
        public int Pushes { get; set; }

	    /// <summary>
	    /// Creates a new player, initializing their bet to 0 and balance to $1,000
	    /// </summary>
	    /// <param name="playerId">The player identifier</param>
	    /// <param name="name">Name of the bot</param>
	    /// <param name="key">Registration key</param>
	    public PlayerState(string playerId, string name, string key)
		{
			PlayerId = playerId;
			Name = name;
			Key = key;
			Bet = 0;
			Balance = 1000;
		}
        /// <summary>
        /// Returns a copy of the current player.
        /// </summary>
        /// <returns>A copy of the PlayerState</returns>
        public PlayerState Clone()
        {
            return (PlayerState)this.MemberwiseClone(); 
        }

        /// <summary>
        /// A string list of name/value pair for the properties of that player
        /// </summary>
        /// <returns>A string representation of the PlayerState</returns>
        public override string ToString()
        {
            return TypeHelper.GetTypes(this); 
        }
	}
}
