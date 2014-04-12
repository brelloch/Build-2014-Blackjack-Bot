using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using BlackjackBot.Bot;
using BlackjackBot.Shared;
using Microsoft.AspNet.SignalR.Client;

namespace BlackjackBot.Wpf.Pages
{
	/// <summary>
	/// Interaction logic for SoloPage.xaml
	/// </summary>
	public partial class VisualizerPage : Page
	{
		private const string Url = "http://blackjackbotserver.azurewebsites.net/";

		private readonly CustomBot _customBot = new CustomBot();
		private readonly bool _solo;
		private readonly List<GameState> _games = new List<GameState>();
		private GameState _lastGameState = new GameState { GameNumber = 1 };
		private int _gameNumber;
		private IHubProxy _hubProxy;

		public VisualizerPage(bool solo)
		{
			InitializeComponent();
			_solo = solo;
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				HubConnection connection = new HubConnection(Url);
				_hubProxy = connection.CreateHubProxy("BlackjackHub");
				_hubProxy.On<List<LeaderboardEntries>>("receiveLeaderboard", ReceiveLeaderboard);
				await connection.Start();            
			}
			catch(Exception ex)
			{
				ShowError("Could not connect to Blackjack Bot server", ex);
				return;
			}

			try
			{
				_customBot.GameStateUpdated += _customBot_GameStateUpdated;
				await _customBot.InitializeAsync(Url);
			}
			catch(Exception ex)
			{
				ShowError("Could not initialize bot.", ex);
			}

			RunGame.IsEnabled = true;
		}

		private void ShowError(string msg, Exception ex)
		{
			MessageBox.Show(msg + Environment.NewLine + Environment.NewLine + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void ReceiveLeaderboard(List<LeaderboardEntries> leaderboardEntries)
		{
			try
			{
				Dispatcher.Invoke(() => Leaderboard.DataContext = leaderboardEntries);
			}
			catch
			{
				// we don't care
			}
		}

		private async void RunGame_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				_games.Clear();
				_lastGameState = new GameState { GameNumber = 1 };

				if(_solo)
					await _customBot.StartSoloGameSeriesAsync(10);
				else
					await _customBot.JoinMultiPlayerGameQueueAsync();

			}
			catch(Exception ex)
			{
				ShowError("Error starting game/joining queue", ex);
			}
		}

		void _customBot_GameStateUpdated(object sender, GameState gameState)
		{
			if( _lastGameState.GameNumber != gameState.GameNumber)
				_games.Add(_lastGameState);
			else if(gameState.GameNumber == gameState.TotalGames)
			{
				// if it's the last game, always save the state, but delete the previous version of it every time
				if(_games.Count == gameState.GameNumber)
					_games.RemoveAt(_games.Count-1);

				_games.Add(gameState);
			}

			_lastGameState = gameState;
			_gameNumber = gameState.GameNumber-1;

			SetGameState(gameState);
		}

		private void Back_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}

		private void NextGame_Click(object sender, RoutedEventArgs e)
		{
			if(_gameNumber < _games.Count - 1)
				SetGameState(_games[++_gameNumber]);
		}

		private void PrevGame_Click(object sender, RoutedEventArgs e)
		{
			if(_gameNumber > 0)
				SetGameState(_games[--_gameNumber]);
		}

		private void SetGameState(GameState gameState)
		{
			try
			{
				Dispatcher.Invoke(() => { DataContext = gameState; });

			}
			catch
			{
				// don't care
			}
		}

		private void ViewGameLog_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(".");
		}
	}
}
