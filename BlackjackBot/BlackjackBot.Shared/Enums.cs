namespace BlackjackBot.Shared
{
	/// <summary>
	/// Card suit values
	/// </summary>
	public enum Suit
	{
        /// <summary>
        /// A diamond card type
        /// </summary>
		Diamonds, 
        
        /// <summary>
        /// A spade card type
        /// </summary>
        Spades, 
        
        /// <summary>
        /// A club card type
        /// </summary>
        Clubs, 
        
        /// <summary>
        /// A heart card type
        /// </summary>
        Hearts
	}

	/// <summary>
	/// A unique identifier for cards. This reprents the card value for numbered cards. 
	/// </summary>
	public enum FaceValue
	{
        /// <summary>
        /// Two
        /// </summary>
		Two = 2, 
        
        /// <summary>
        /// Three
        /// </summary>
        Three = 3, 
        
        /// <summary>
        /// Four
        /// </summary>
        Four = 4, 
        
        /// <summary>
        /// Five
        /// </summary>
        Five = 5, 
        
        /// <summary>
        /// Six
        /// </summary>
        Six = 6, 
        
        /// <summary>
        /// Seven
        /// </summary>
        Seven = 7, 
        
        /// <summary>
        /// Eight
        /// </summary>
        Eight = 8,
		
        /// <summary>
        /// Nine
        /// </summary>
        Nine = 9, 
        
        /// <summary>
        /// Ten
        /// </summary>
        Ten = 10, 
        
        /// <summary>
        ///  Jack/Eleven
        /// </summary>
        Jack = 11, 
        
        /// <summary>
        /// Queen/12
        /// </summary>
        Queen = 12, 
        
        /// <summary>
        /// King/13
        /// </summary>
        King = 13, 
        
        /// <summary>
        /// Ace/14
        /// </summary>
        Ace = 14
	}

    /// <summary>
    /// The set of legal moves that can be sent to the Blackjack Game Service.
    /// </summary>
	public enum Move
	{
        /// <summary>
        /// When sent as a move, will give you another card. If the total value of your hand is less than 21, PlayerMove will fire. 
        /// If you bust or get 21, you will not receive another PlayerMove event. 
        /// </summary>
		Hit					= 0,

        /// <summary>
        /// When a player stands, it will be the next players turn. 
        /// </summary>
		Stand				= 1,

        /// <summary>
        /// You can only double down if you have not yet received another card. Doubling down means you receive exactly one card, and your bet amount is doubled. You must have a balance large enough to cover the double down.
        /// For example, if a player has a balance of $1,000. If you bet $500, you can double down ($500 x 2 less than balance of $1,000)
        /// If you bet $750 and your balance is $1,000, you will receive an error saying they do not have a balance, ($1000 is less than $750 x 2 or $1,500)
        /// </summary>
		DoubleDown			= 2,

        /// <summary>
        /// By default, you can only see one of the dealer’s card. If you pay for a dealer’s card, this must be the first thing you do on your turn. 
        /// After placing this move, it will be your turn again. If you pay to see the dealer’s card, you will only receive 75% of your bet. 
        /// For example, with a bet of $1,000, if you win, you would only keep $750. 
        /// </summary>
        PayToSeeDealersCard	= 3
	}



	/// <summary>
	/// EndResult maintains the game result state. 
	/// </summary>
	public enum EndResult
	{
        /// <summary>
        /// The dealer got Blackjack. In this case, you will not get a PlayerMove event. This is a loss. 
        /// If a player gets a Blackjack when a Dealer gets a Blackjack the result will be a push (tie) instead. 
        /// </summary>
		DealerBlackJack	    = 0,

        /// <summary>
        /// The player got Blackjack and will be paid 3:2, so for a bet of $100, you will win $150. This is a win. 
        /// </summary>
		PlayerBlackJack	    = 1,

        /// <summary>
        /// The player has busted by going over 21. This is a loss. 
        /// </summary>
		PlayerBust		    = 2,

        /// <summary>
        /// The dealer has busted by going over 21. This is a win. 
        /// </summary>
		DealerBust		    = 3,

        /// <summary>
        /// The player has tied with the dealer. This is a push (tie)
        /// </summary>
		Push			    = 4,

        /// <summary>
        /// The player won the game. 
        /// </summary>
		PlayerWin		    = 5,

        /// <summary>
        /// The dealer won the game. 
        /// </summary>
		DealerWin		    = 6,

        /// <summary>
        /// The player has timed out by not sending a valid PlayerMove within two seconds. This is a loss. 
        /// </summary>
        PlayerMoveTimeout   = 7
	}
}
