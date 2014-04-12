using BlackjackBot.Shared;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlackjackBot.Bot
{
	public class CustomBot : IBot
	{
        /// <summary>
        /// A unique key required to run your bot. The key can be obtained by registering.
        /// </summary>
        private const string Key = "a045cbbd6f4e4b29abe82392bba4cb4c";
        public CustomBot()
        {
            //make sure to register your bot to get a valid name and key!
            BotName = "gth685f";
        }

        /// <summary>
        /// A unique name for the bot
        /// </summary>
        public string BotName { get; private set; }

		//SignalR Hub proxy
		private IHubProxy _hubProxy;

        int cardCount = 0;

        /// <summary>
        /// This event will fire when the Game State is updated either with a new game, a move, or a game is completed
        /// </summary>
		public event EventHandler<GameState> GameStateUpdated;

		/// <summary>
		/// A list of cards that have been played. 
		/// </summary>
		readonly List<Card> _cardsPlayed = new List<Card>();

		/// <summary>
		/// Will be set to true when the GameSeries completed event is fired and set to false when a series is starting. 
		/// </summary>
		public bool IsGameSeriesCompleted { get; private set; }

		/// <summary>
		/// This sets up your signalRbot
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public async Task InitializeAsync(string url)
		{
			//Sets up connection and events from SignalR server
			HubConnection connection = new HubConnection(url);
			_hubProxy = connection.CreateHubProxy("BlackjackHub");

			//Receive when Game Starts
			_hubProxy.On<GameState>("gameStarted", GameStarted);

			//Receive when it's your turn
			_hubProxy.On<GameState>("playerMove", async gameState => { await PlayerMoveAsync(gameState); });

			//Receive when game is completed
			_hubProxy.On<GameState>("gameCompleted", GameCompleted);

			//Receive when game series is completed
			_hubProxy.On<GameState>("gameSeriesCompleted", GameSeriesCompleted);

			//Receive when cards are shuffled
			_hubProxy.On("deckShuffled", DeckShuffled);

			//Receive when an error is received
			_hubProxy.On<string>("errorReceived", ErrorReceived);
			_hubProxy.On<string>("informationReceived", InformationReceived);

			//setup SignalR
			await connection.Start();            
		}

		/* START METHODS TO START BLACKJACK  */
		public async Task<bool> JoinMultiPlayerGameQueueAsync()
		{
			bool added = await _hubProxy.Invoke<bool>("JoinMultiPlayerGameQueueAsync", BotName, Key);
			if(added)
				Debug.WriteLine(BotName + " joined multiplayer game queue"); 
			else
				Debug.WriteLine(BotName + " is already in the queue or used an invalid name/key combination");
			return added;
		}

		public async Task<bool> StartSoloGameSeriesAsync(int numberOfGames)
		{
			//validate # of games is between 1 and 10 (this is done on the server too)
			if (numberOfGames <= 0)
				return false;

			if (numberOfGames > 10)
				numberOfGames = 10;

			bool started = await _hubProxy.Invoke<bool>("StartSoloGameSeriesAsync", BotName, Key, numberOfGames);
			if(started)
				Debug.WriteLine("SoloGameSeries Started");
			else
				Debug.WriteLine("Failed to start solo game");
			return started;
		}
        /* END METHODS TO PLAY BLACKJACK */


        /* START METHODS FOR YOU TO CUSTOMIZE */
		private void GameStarted(GameState gameState)
		{
            //Fire the event to share the new GameState
			OnGameStateUpdated(gameState);

			//no data returned
			if (gameState.Me == null)
				return;

			// balance must be positive
			if (gameState.Me.Balance <= 0)            
				return;             

			try
			{
                decimal amountToBet = GameState.MinimumBet * 4;

                if (cardCount == 1)
                    amountToBet = amountToBet * 2;
                else if (cardCount == 2)
                    amountToBet = amountToBet * 4;
                else if (cardCount == 3)
                    amountToBet = amountToBet * 8;
                else if (cardCount == 4)
                    amountToBet = amountToBet * 10;
                else if (cardCount > 4)
                    amountToBet = amountToBet * 12;

                if (amountToBet > gameState.Me.Balance)
                    amountToBet = GameState.MinimumBet;
                else if (amountToBet > gameState.Me.Balance/3) // block bets that use excessive amounts of cash
                {
                    amountToBet = gameState.Me.Balance / 3;
                }

                if (amountToBet > GameState.MaximumBet)
                {
                    amountToBet = GameState.MaximumBet;
                }

                amountToBet = GameState.MaximumBet;

                if (amountToBet > gameState.Me.Balance)
                {
                    amountToBet = gameState.Me.Balance;
                }

                if (gameState.Me.Balance > 40000)
                {
                    amountToBet = GameState.MinimumBet;
                }
                
				if (amountToBet >= GameState.MinimumBet && amountToBet <= GameState.MaximumBet)
				{
					//place your bet
					_hubProxy.Invoke<decimal>("PlaceBet", amountToBet);
					Debug.WriteLine("Bot:" + gameState.Me.Name + ", Placed Bet:" + amountToBet);
				}
				else
				{
					Debug.WriteLine("Bot:" + gameState.Me.Name + ", Bet must be between:" + GameState.MinimumBet + " and " + GameState.MaximumBet);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("no bet placed, exception:" + ex); 
			}    
		}

		private async Task PlayerMoveAsync(GameState gameState)
		{
            //Fire the event to share the new GameState
			OnGameStateUpdated(gameState);

			if (gameState.Me.Hand == null)
			{
				Debug.WriteLine(gameState.Me.Name + " has null hand");
				return; 
			}

            //the move we will be sending
			Move moveToSend;

            if (gameState.Me.PaidToSeeDealersCard == false)
            {
                moveToSend = Move.PayToSeeDealersCard;
            }
            else
            {
                //Get the best hand without busting
                int total = gameState.Me.Hand.GetBestHand();
                var dealerTotal = gameState.AllPlayers.Where(x => x.PlayerId.Contains("Dealer")).FirstOrDefault().Hand.GetBestHand();

                var softHand = false;
                var possibleHands = gameState.Me.Hand.GetSumOfHand();
                if (possibleHands.Where(x => x < 22).Count() > 1 && total < 17 && possibleHands[0] != possibleHands[1])
                {
                    softHand = true;
                }

                var dealerSoftHand = false;
                var dealerPossibleHands = gameState.AllPlayers.Where(x => x.PlayerId.Contains("Dealer")).FirstOrDefault().Hand.GetSumOfHand();
                if (dealerPossibleHands.Where(x => x < 22).Count() > 1 && dealerTotal < 17 && dealerPossibleHands[0] != dealerPossibleHands[1])
                {
                    dealerSoftHand = true;
                }

                if (!softHand)
                {
                    if (!dealerSoftHand) // player and dealer both hard
                    {
                        if ((total > 7 && total < 12 && dealerTotal > 11 && dealerTotal < 17) ||
                             (total > 4 && total < 8 && dealerTotal > 13 && dealerTotal < 17) ||
                             (total > 9 && total < 12 && dealerTotal > 3 && dealerTotal < 9) ||
                             (total == 11 && dealerTotal == 9) ||
                             (total == 9 && dealerTotal == 5) ||
                             (total == 9 && dealerTotal == 6))
                        {
                            moveToSend = Move.Hit;
                        }
                        else if ((total < 11) ||
                                (total > 11 && total < 14 && dealerTotal > 6 && dealerTotal < 12) ||
                                (total > 13 && total < 16 && dealerTotal > 6 && dealerTotal < 10) ||
                                (total == 14 && dealerTotal == 10) ||
                                (total == 16 && dealerTotal ==  7) ||
                                (dealerTotal > 16 && dealerTotal > total))
                        {
                            moveToSend = Move.Hit;
                        }
                        else 
                        {
                            moveToSend = Move.Stand;
                        }
                    }
                    else // player hard dealer soft
                    {
                        if ((total > 9 && total < 12 && dealerTotal > 13 && dealerTotal < 17) ||
                            (total == 11 && dealerTotal == 13))
                        {
                            moveToSend = Move.Hit;
                        }
                        else if ((total < 12) ||
                                 (total == 12 && dealerTotal > 11 && dealerTotal < 14) ||
                                 (dealerTotal > 16 && dealerTotal > total))
                        {
                            moveToSend = Move.Hit;
                        } else {
                            moveToSend = Move.Stand;
                        }
                    }
                }
                else
                {
                    if (!dealerSoftHand) // player soft and dealer hard
                    {
                        if ((total < 19 && dealerTotal > 11 && dealerTotal < 17) ||
                            (total == 20 && dealerTotal > 12 && dealerTotal < 17) ||
                            (total > 13 && total < 19 && dealerTotal == 6) ||
                            (total > 15 && total < 19 && dealerTotal == 5) ||
                            (total == 18 && dealerTotal == 4))
                        {
                            moveToSend = Move.Hit;
                        } else if ((total < 16) ||
                                   (total < dealerTotal && dealerTotal > 16)) {
                            moveToSend = Move.Hit;
                        } else {
                            moveToSend = Move.Stand;
                        }
                    }
                    else // player and dealer both soft
                    {
                        if ((total < 17) ||
                            (total < dealerTotal && dealerTotal > 16))
                        {
                            moveToSend = Move.Hit;
                        }
                        else
                        {
                            moveToSend = Move.Stand;
                        }
                    }
                }

            }
			//Send our move
			await _hubProxy.Invoke<Move>("PlayerMoveAsync", moveToSend);
		}

        private void DeckShuffled()
        {
            Debug.WriteLine("DeckShuffled fired");
            //clear out my cards; 
            _cardsPlayed.Clear();
            cardCount = 0;
        }

		private void GameCompleted(GameState gameState)
		{
            //Fire the event to share the new GameState
			OnGameStateUpdated(gameState);

            Debug.WriteLine("My cards: " + string.Join(",", gameState.Me.Hand.Cards.Select(x => x.FaceVal.ToString())));
            Debug.WriteLine("Dealer cards: " + string.Join(",", gameState.AllPlayers.Where(x => x.PlayerId.Contains("Dealer")).FirstOrDefault().Hand.Cards.Select(x => x.FaceVal.ToString())));
            Debug.WriteLine("End of Game\n");           
			Debug.WriteLine(gameState.DisplayResults()); 

			//loop through all players and add the used cards to List
			foreach (var player in gameState.AllPlayers)
			{
				foreach (var card in player.Hand.Cards)
				{
					_cardsPlayed.Add(card);
				}
			}

            var lowCards = _cardsPlayed.Where(x => x.FaceVal == FaceValue.Two ||
                                                x.FaceVal == FaceValue.Three ||
                                                x.FaceVal == FaceValue.Four ||
                                                x.FaceVal == FaceValue.Five ||
                                                x.FaceVal == FaceValue.Six).Count();

            var highCards = _cardsPlayed.Where(x => x.FaceVal == FaceValue.Ace ||
                                                x.FaceVal == FaceValue.King ||
                                                x.FaceVal == FaceValue.Queen ||
                                                x.FaceVal == FaceValue.Jack ||
                                                x.FaceVal == FaceValue.Ten).Count();
            cardCount = lowCards - highCards;
		}

		private void GameSeriesCompleted(GameState gameState)
		{
			OnGameStateUpdated(gameState);

			IsGameSeriesCompleted = true;
            PlayerState winner = gameState.AllPlayers.OrderByDescending(p => p.Balance).FirstOrDefault(); 
			Debug.WriteLine("End of Game Series, winner:" + winner.Name + ", Balance:" + winner.Balance);
		}

        private void InformationReceived(string infoMessage)
        {
            Debug.WriteLine("Info: " + infoMessage);
        }

        private void ErrorReceived(string errorMessage)
        {
            Debug.WriteLine("** Error: " + errorMessage);
        }

        /* END METHODS FOR YOU TO CUSTOMIZE */

		//Use this diagnostic method to ensure that you're getting results back
		public async Task<string> CallEchoTest(string text)
		{
			//calls the service and returns the result instantly
			string result = await _hubProxy.Invoke<string>("Echo", text);
			Debug.WriteLine(result);
			return result;
		}
        //Fires the GameStateUpdated event when it receives new GameState
		private void OnGameStateUpdated(GameState gameState)
		{
			if(GameStateUpdated != null)
				GameStateUpdated(this, gameState);
		}  
	}
}
