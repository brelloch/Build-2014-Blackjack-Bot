namespace BlackjackBot.Shared
{
    /// <summary>
    /// A card in the blackjack game (ex: Ace of Spades)
    /// </summary>
    public class Card
    {
        // Objects for card information
        private readonly Suit _suit;
        private readonly FaceValue _faceVal;

        /// <summary>
        /// The suit of the card
        /// </summary>
        public Suit Suit { get { return _suit; } }
        
        /// <summary>
        /// The unique ID or value of the card. 
        /// </summary>
        public FaceValue FaceVal { get { return _faceVal; } }


        /// <summary>
        /// Constructor for a new card.  Assign the card a suit, face value, and if the card is facing up or down
        /// </summary>
        /// <param name="suit">An enumeration representing the suit for a card</param>
        /// <param name="faceVal">the value or unique identifier for a card</param>
        /// 
        public Card(Suit suit, FaceValue faceVal)
        {
            _suit = suit;
            _faceVal = faceVal;
        }

        /// <summary>
        /// Return the card as a string (i.e. "The Ace of Spades")
        /// </summary>
        /// <returns>A string representation of the card</returns>
        public override string ToString()
        {
            return _faceVal + "Of" + _suit;
        }
    }
}