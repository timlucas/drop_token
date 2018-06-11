using DropTokenGame.GameExceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame.Tests
{
    [TestFixture]
    public class GameKeeperTests
    {
        private List<String> players;

        [SetUp]
        public void Setup()
        {
            players = new List<string>();
            players.Add("player1");
            players.Add("player2");
        }

        [Test]
        public void CanAddGamesWithExpectedIds()
        {
            var gameId1 = GameKeeper.AddGame(4, 4, players);
            var gameId2 = GameKeeper.AddGame(4, 4, players);

            Assert.NotNull(gameId1);
            Assert.NotNull(gameId2);
            Assert.AreNotEqual(string.Empty, gameId1);
            Assert.AreNotEqual(string.Empty, gameId2);
            Assert.AreEqual("Game0", gameId1);
            Assert.AreEqual("Game1", gameId2);
        }

        [Test]
        public void CanGetGameIds()
        {
            var gameId1 = GameKeeper.AddGame(4, 4, players);
            var gameId2 = GameKeeper.AddGame(4, 4, players);
            var gameIds = GameKeeper.GetGameIds();

            Assert.IsNotEmpty(gameIds);
            Assert.Contains(gameId1, gameIds);
            Assert.Contains(gameId2, gameIds);
        }
        
        [Test]
        public void CanGetGameState()
        {
            var gameId1 = GameKeeper.AddGame(4, 4, players);
            var gameState = GameKeeper.GetGameState(gameId1);

            Assert.IsNotNull(gameState);
            Assert.AreEqual(Game.GameStates.IN_PROGRESS, gameState.State);
            Assert.IsNull(gameState.Winner);
            Assert.AreEqual(players, gameState.Players);
        }
        
        [Test]
        public void GettingInvalidGameStateReturnsNull()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var gameState = GameKeeper.GetGameState("bogusId");
            });
        }

        [Test]
        public void GetGameMovesIsEmptyForNewGame()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var moves = GameKeeper.GetGameMoves(newGameId);
            Assert.IsEmpty(moves);
        }

        [Test]
        public void CanGetAllGameMoves()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));

            var moves = GameKeeper.GetGameMoves(newGameId);
            Assert.AreEqual(4, moves.Count());
        }

        [Test]
        public void CanGetFirstTwoGameMoves()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));

            var moves = GameKeeper.GetGameMoves(newGameId, 0, 1);
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual(0, ((TokenMove)moves.First()).Column);
            Assert.AreEqual(1, ((TokenMove)moves.Last()).Column);
        }

        [Test]
        public void CanGetFirstTwoGameMovesWithoutSpecifyingStartParam()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));

            var moves = GameKeeper.GetGameMoves(newGameId, null, 1);
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual(0, ((TokenMove)moves.First()).Column);
            Assert.AreEqual(1, ((TokenMove)moves.Last()).Column);
        }

        [Test]
        public void CanGetLastTwoGameMoves()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));

            var moves = GameKeeper.GetGameMoves(newGameId, 2, 3);
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual(0, ((TokenMove)moves.First()).Column);
            Assert.AreEqual(1, ((TokenMove)moves.Last()).Column);
        }

        [Test]
        public void CanGetLastTwoGameMovesWithoutSpecifyingUntilParam()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));

            var moves = GameKeeper.GetGameMoves(newGameId, 2);
            Assert.AreEqual(2, moves.Count());
            Assert.AreEqual(0, ((TokenMove)moves.First()).Column);
            Assert.AreEqual(1, ((TokenMove)moves.Last()).Column);
        }

        [Test]
        public void CanGetFirstGameMove()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));

            var move = GameKeeper.GetGameMove(newGameId, move1);
            Assert.AreEqual(players[0], move.Player);
            Assert.AreEqual(DropTokenGame.MoveTypes.MOVE, move.MoveType);
            Assert.AreEqual(0, ((TokenMove)move).Column);
        }

        [Test]
        public void CanPostMoves()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 0));

            Assert.AreEqual(0, move1);
            Assert.AreEqual(1, move2);

            var moves = GameKeeper.GetGameMoves(newGameId);
            Assert.AreEqual(2, moves.Count());
        }

        [Test]
        public void SamePlayerCantPlayTwoTokenMovesInARow()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));

            Assert.Throws<OutOfTurnException>(new TestDelegate(() =>
            {
                GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            }));
        }

        [Test]
        public void PuttingTokenInFullColumnWillThrow()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 0));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 0));

            // the first column should now be full...try one more
            Assert.Throws<IllegalMoveException>(() => {
                var move5 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            });
        }

        [Test]
        public void ColumnWinRegistersProperly()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move5 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move6 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            
            // each player should have 3 tokens in a row - player 0 in column 0 should win with one more token there...

            var beforeMoveGameState = GameKeeper.GetGameState(newGameId);
            var move7 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var afterMoveGameState = GameKeeper.GetGameState(newGameId);

            Assert.AreEqual(Game.GameStates.IN_PROGRESS, beforeMoveGameState.State);
            Assert.IsNull(beforeMoveGameState.Winner);
            Assert.AreEqual(Game.GameStates.DONE, afterMoveGameState.State);
            Assert.AreEqual(players[0], afterMoveGameState.Winner);
        }

        [Test]
        public void RowWinRegistersProperly()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 0));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 1));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move5 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 2));
            var move6 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 2));

            // each player should have 3 tokens on the board - player 0 in column 3 should win with one more token there...

            var beforeMoveGameState = GameKeeper.GetGameState(newGameId);
            var move7 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 3));
            var afterMoveGameState = GameKeeper.GetGameState(newGameId);

            Assert.AreEqual(Game.GameStates.IN_PROGRESS, beforeMoveGameState.State);
            Assert.IsNull(beforeMoveGameState.Winner);
            Assert.AreEqual(Game.GameStates.DONE, afterMoveGameState.State);
            Assert.AreEqual(players[0], afterMoveGameState.Winner);
        }

        [Test]
        public void MinorDiagonalWinRegistersProperly()
        {
            /*   | | | |0|
             *   | | |0|0|
             *   |1|0|1|1|
             *   |0|1|1|0|
             * */

            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 1));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 2));
            var move5 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 3));
            var move6 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 2));
            var move7 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 2));
            var move8 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 3));
            var move9 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 3));
            var move10 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 0));

            var beforeMoveGameState = GameKeeper.GetGameState(newGameId);
            var move11 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 3));
            var afterMoveGameState = GameKeeper.GetGameState(newGameId);

            Assert.AreEqual(Game.GameStates.IN_PROGRESS, beforeMoveGameState.State);
            Assert.IsNull(beforeMoveGameState.Winner);
            Assert.AreEqual(Game.GameStates.DONE, afterMoveGameState.State);
            Assert.AreEqual(players[0], afterMoveGameState.Winner);
        }

        [Test]
        public void MajorDiagonalWinRegistersProperly()
        {
            /*   |1| | | |
             *   |0|1| |0|
             *   |1|0|1|0|
             *   |0|1|0|1|
             * */

            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move2 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move3 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 2));
            var move4 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 3));
            var move5 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 1));
            var move6 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 0));
            var move7 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 3));
            var move8 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 2));
            var move9 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            var move10 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 1));
            var move11 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 3));

            var beforeMoveGameState = GameKeeper.GetGameState(newGameId);
            var move12 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[1], 0));
            var afterMoveGameState = GameKeeper.GetGameState(newGameId);

            Assert.AreEqual(Game.GameStates.IN_PROGRESS, beforeMoveGameState.State);
            Assert.IsNull(beforeMoveGameState.Winner);
            Assert.AreEqual(Game.GameStates.DONE, afterMoveGameState.State);
            Assert.AreEqual(players[1], afterMoveGameState.Winner);
        }

        [Test]
        public void PlayerCanQuitEvenWhenNotThatPlayersTurn()
        {
            var newGameId = GameKeeper.AddGame(4, 4, players);
            var move1 = GameKeeper.RegisterMove(newGameId, new TokenMove(players[0], 0));
            
            var beforeMoveGameState = GameKeeper.GetGameState(newGameId);
            var move2 = GameKeeper.RegisterMove(newGameId, new QuitMove(players[0]));
            var afterMoveGameState = GameKeeper.GetGameState(newGameId);

            Assert.AreEqual(Game.GameStates.IN_PROGRESS, beforeMoveGameState.State);
            Assert.IsNull(beforeMoveGameState.Winner);
            Assert.AreEqual(Game.GameStates.DONE, afterMoveGameState.State);
            Assert.AreEqual(players[1], afterMoveGameState.Winner);     // the player that didn't quit is the winner
        }
    }
}
