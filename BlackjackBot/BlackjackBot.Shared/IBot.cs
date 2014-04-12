using System;
using System.Threading.Tasks;

namespace BlackjackBot.Shared
{
    /// <summary>
    /// The interface that all AI bots must implement
    /// </summary>
    public interface IBot
    {
        /// <summary>
        /// InitializeAsync is used to setup a bot.
        /// </summary>
        /// <param name="url">The Url of the SignalR service</param>
        /// <returns>A task</returns>
        Task InitializeAsync(string url);

        /// <summary>
        /// Use to join a multiplayer game queue
        /// </summary>
        /// <returns>A task</returns>
        Task<bool> JoinMultiPlayerGameQueueAsync();

		/// <summary>
		/// Use to start a solo game
		/// </summary>
		/// <param name="numberOfGames">Number of games to play in the series</param>
		/// <returns>A task</returns>
		Task<bool> StartSoloGameSeriesAsync(int numberOfGames);

		/// <summary>
		/// Event used to update the game's visualzier
		/// </summary>
		event EventHandler<GameState> GameStateUpdated;
    }
}
