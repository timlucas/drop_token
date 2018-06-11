using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame
{
    public static class GameKeeper
    {
        private const int numberOfPiecesToWin = 4;
        private static ConcurrentDictionary<string, Game> gamesInProgress = new ConcurrentDictionary<string, Game>();   // keep this collection smaller
        private static ConcurrentDictionary<string, Game> finishedGames = new ConcurrentDictionary<string, Game>();
        private static object gameListLock = new object();

        //public static GameKeeper()
        //{
        //    gamesInProgress = new ConcurrentDictionary<string, Game>();
        //    finishedGames = new ConcurrentDictionary<string, Game>();
        //}

        /// <summary>
        /// Add a new game
        /// </summary>
        /// <param name="players">A list of players for the game (there must be exactly 2 players)</param>
        /// <returns></returns>
        public static string AddGame(int rows, int columns, List<String> players)
        {
            if (players == null || players.Count != 2)
            {
                throw new ArgumentException("Games must have exactly 2 players");
            }

            var i = 0;
            foreach (string player in players)
            {
                if (String.IsNullOrWhiteSpace(player))
                {
                    throw new ArgumentException(String.Format("Player {0} must have non-empty name", i));
                }
                i++;
            }
            if (rows <= 0)
            {
                throw new ArgumentException("Please specify a row count greater than 0", "rows");
            }
            if (columns <= 0)
            {
                throw new ArgumentException("Please specify a column count greater than 0", "columns");
            }

            String gameId = null;

            lock (gameListLock)
            {
                // lock the list while we're trying to get a new ID that doesn't yet exist
                gameId = getUnusedId();
                Game newGame = new Game(gameId, columns, rows, players.ToArray(), numberOfPiecesToWin);
                gamesInProgress.TryAdd(gameId, newGame);
            }

            return gameId;
        }

        private static String getUnusedId() {
            int gameCount = gamesInProgress.Count + finishedGames.Count;
            String tryGameStub = "Game";

            while (gamesInProgress.ContainsKey(tryGameStub + gameCount.ToString())) {
                gameCount += 1;
            }

            return tryGameStub + gameCount.ToString();
        }

        /// <summary>
        /// Get the IDs of games still in progress
        /// </summary>
        public static List<String> GetGameIds() {
            // without a separate collection for finished games, we'd need to hit every game in the (single) collection to find those still in progress
            return gamesInProgress.Keys.ToList();
        }

        /// <summary>
        /// Get the specified game
        /// </summary>
        /// <param name="gameId">ID of the game in question</param>
        private static Game getGame(string gameId)
        {
            if (gamesInProgress.ContainsKey(gameId))
            {
                return gamesInProgress[gameId];
            }
            
            if (finishedGames.ContainsKey(gameId))
            {
                return finishedGames[gameId];
            }

            return null;
        }

        /// <summary>
        /// Get the state of the specified game
        /// </summary>
        /// <param name="gameId">ID of the game in question</param>
        public static GameState GetGameState(string gameId) {
            Game game = getGame(gameId);

            if (game == null)
            {
                throw new KeyNotFoundException(string.Format("Game with ID {0} not found.", gameId));
            }

            return new GameState(game);
        }

        /// <summary>
        /// Get moves for the specified game
        /// </summary>
        /// <param name="gameId">ID of the game in question</param>
        /// <param name="start">Start index (0-based); if not supplied, will start at 0</param>
        /// <param name="end">End index (0-based); if not supplied, will end at last move</param>
        public static IEnumerable<Move> GetGameMoves(string gameId, int? start = null, int? end = null)
        {
            var game = getGame(gameId);

            if (game == null)
            {
                throw new KeyNotFoundException(string.Format("Game with ID {0} not found.", gameId));
            }

            if (game.Moves == null || game.Moves.Count == 0)
            {
                throw new ApplicationException("Game moves not found!");
            }

            int startIndex = 0;
            int endIndex = game.Moves.Count - 1;

            if (start.HasValue)
            {
                if (start.Value < 0 || start.Value >= game.Moves.Count)
                {
                    throw new IndexOutOfRangeException(String.Format("Start index must be between 0 and {0}", (game.Moves.Count - 1)));
                }
                else
                {
                    startIndex = start.Value;
                }
            }

            if (end.HasValue)
            {
                if (end.Value < 0 || end.Value >= game.Moves.Count)
                {
                    throw new IndexOutOfRangeException(String.Format("End index must be between 0 and {0}", (game.Moves.Count - 1)));
                }
                else if (end.Value < startIndex)
                {
                    throw new ArgumentException("End index must be greater than or equal to the start index");
                }
                else
                {
                    endIndex = end.Value;
                }
            }

            return game.Moves.ToList()
                             .Skip(startIndex)
                             .Take((endIndex+1) - startIndex);
        }

        /// <summary>
        /// Get the specified game move
        /// </summary>
        /// <param name="gameId">ID of the game in question</param>
        /// <param name="moveNumber">Move number (0-based)</param>
        public static Move GetGameMove(string gameId, int moveNumber)
        {
            var game = getGame(gameId);

            if (game == null)
            {
                throw new KeyNotFoundException(string.Format("Game with ID {0} not found.", gameId));
            }

            if (game.Moves == null)
            {
                throw new ApplicationException("Game moves not found!");
            }

            if (moveNumber < 0 || moveNumber >= game.Moves.Count)
            {
                throw new ArgumentOutOfRangeException("moveNumber", "Invalid move number");
            }

            return game.Moves[moveNumber];
        }

        /// <summary>
        /// Make a move in the game
        /// </summary>
        /// <param name="gameId">ID of the game in question</param>
        /// <param name="move">A move by one of the players in the game (placing a token, or quitting)</param>
        /// <returns>Returns the move number (if successful; throws exception if something is wrong)</returns>
        public static int RegisterMove(string gameId, Move move)
        {
            int moveNumber = -1;
            Game game = null;

            lock (gameId)   // don't allow other processes to change the same game at the same time
            {
                game = getGame(gameId);

                if (game != null)
                {
                    moveNumber = game.PerformMove(move);

                    if (game.GameState == Game.GameStates.DONE)
                    {
                        // move the game to the "finished" collection
                        if (gamesInProgress.TryRemove(gameId, out game))
                        {
                            finishedGames.TryAdd(gameId, game);
                        }
                    }
                }
            }

            if (game == null)
            {
                throw new KeyNotFoundException(string.Format("Game with ID {0} not found.", gameId));
            }

            return moveNumber;
        }
    }
}
