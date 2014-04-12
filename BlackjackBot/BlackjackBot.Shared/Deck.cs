using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BlackjackBot.Shared
{
    /// <summary>
    /// A deck represents a collection of cards. This is not used in a bot, but is included so developers can see the implementation.
    /// </summary>
	public class Deck
	{
        /// <summary>
        /// An event that fires when the deck is being shuffled.
        /// </summary>
		public event EventHandler<EventArgs> DeckShuffled;

		private int _totalCards;

        /// <summary>
        /// Total number of decks (52 cards per deck)
        /// </summary>
        public int NumberOfDecks { get; set; }

		// Creates a list of cards
		private readonly List<Card> _cards = new List<Card>();

		/// <summary>
		/// Creates classic 52 cards with each suit and face value
		/// </summary>
		/// <param name="numberOfDecks">How many decks to create</param>
		public Deck(int numberOfDecks)
		{
			if(numberOfDecks <= 0)
				throw new ArgumentOutOfRangeException("numberOfDecks", "numberOfDecks must be > 0");

			NumberOfDecks = numberOfDecks;
			CreateDecks(NumberOfDecks);
		}

        /// <summary>
        /// Rehydrates a serialized version of a deck.
        /// </summary>
        /// <param name="deck">A serialized copy of the deck</param>
		public Deck(string deck)
		{
			if(string.IsNullOrEmpty(deck))
				throw new ArgumentException("deck must contain a valid serialized Deck object", "deck");

			_cards = new List<Card>((deck.Length/2));

			for(int i = 0; i < deck.Length; i+= 2)
			{
				int count;
				FaceValue faceValue = 0;
				Suit suit = 0;

				if(int.TryParse(deck[i].ToString(), out count))
				{
					if(count == 0)
						faceValue = FaceValue.Ten;
					else
						faceValue = (FaceValue)count;
				}
				else
				{
					switch(deck[i])
					{
						case 'J':
							faceValue = FaceValue.Jack;
							break;
						case 'Q':
							faceValue = FaceValue.Queen;
							break;
						case 'K':
							faceValue = FaceValue.King;
							break;
						case 'A':
							faceValue = FaceValue.Ace;
							break;
					}
				}

				switch(deck[i+1])
				{
					case 'H':
						suit = Suit.Hearts;
						break;
					case 'D':
						suit = Suit.Diamonds;
						break;
					case 'C':
						suit = Suit.Clubs;
						break;
					case 'S':
						suit = Suit.Spades;
						break;
				}

				_cards.Add(new Card(suit, faceValue));
			}

			_totalCards = _cards.Count; 
		}

		private void CreateDecks(int numberOfDecks)
		{
			for (int i = 0; i < numberOfDecks; i++)
			{
				foreach (Suit suit in Enum.GetValues(typeof(Suit)))
				{
					foreach (FaceValue faceVal in Enum.GetValues(typeof(FaceValue)))
					{
						_cards.Add(new Card(suit, faceVal));
					}
				}
			}

			//set total # of cards when game starts
			_totalCards = _cards.Count; 

			//shuffle after creating
			this.Shuffle(); 
		}

		/// <summary>
		/// Draws one card and removes it from the deck
		/// </summary>
		/// <returns>Returns a single card</returns>
		public Card Draw()
		{
			//check if we are running out of cards
			if(_cards.Count < _totalCards/10)
			{
				_cards.Clear();
				CreateDecks(NumberOfDecks); 

				//fire event to let people know of reshuffle
				OnDeckShuffled();
			}

			Card card = _cards[0];
			_cards.RemoveAt(0);
			return card;
		}

		/// <summary>
		/// Shuffles the cards in the deck
		/// </summary>
		public void Shuffle()
		{
			Random random = new Random();
			for (int i = 0; i < _cards.Count; i++)
			{
				int index1 = i;
				int index2 = random.Next(_cards.Count);
				SwapCard(index1, index2);
			}
		}

		private void SwapCard(int index1, int index2)
		{
			Card card = _cards[index1];
			_cards[index1] = _cards[index2];
			_cards[index2] = card;
		}

		private void OnDeckShuffled()
		{
			if (DeckShuffled != null)
				DeckShuffled(this, new EventArgs());
		}

        /// <summary>
        /// A string of all cards
        /// </summary>
        /// <returns>A string of all cards</returns>
		public override string ToString()
		{
            StringBuilder sb = new StringBuilder(); 

			foreach(Card c in _cards)
			{
                if (c.FaceVal < FaceValue.Ten)
                    sb.Append(((int)c.FaceVal).ToString());
                else if (c.FaceVal == FaceValue.Ten)
                    sb.Append('0');
                else
                    sb.Append(c.FaceVal.ToString()[0].ToString());

                sb.Append(c.Suit.ToString()[0].ToString());
			}

			return sb.ToString();
		}
	}
}
