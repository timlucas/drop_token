using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame.Tests
{
    [TestFixture]
    public class GameTests
    {
        private string[] players;
        private string gameId = "testgame";
        private int winningPieces = 4;
        private int rows = 4;
        private int columns = 4;

        [SetUp]
        public void Setup()
        {
            players = new string[2];
            players[0] = "player1";
            players[1] = "player2";
        }

        [Test]
        public void CanCreateGameWithTwoPlayers()
        {
            var game = new Game(gameId, 4, 4, players, winningPieces);
            Assert.IsNotNull(game);
        }

        [Test]
        public void NewGameWithOnePlayersThrows()
        {
            var oneplayer = new string[1];
            oneplayer[0] = "onlyme";

            Assert.Throws<ArgumentException>(() =>
            {
                var game = new Game(gameId, 4, 4, oneplayer, winningPieces);
            });
        }

        [Test]
        public void NewGameWithNoPlayersThrows()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var game = new Game(gameId, 4, 4, new string[] {}, winningPieces);
            });
        }

        [Test]
        public void NewGameWithThreePlayersThrows()
        {
            var threeplayers = new string[3];
            threeplayers[0] = "one";
            threeplayers[0] = "two";
            threeplayers[0] = "three";

            Assert.Throws<ArgumentException>(() =>
            {
                var game = new Game(gameId, 4, 4, threeplayers, winningPieces);
            });
        }

        [Test]
        public void GameCanIdentifyValidPlayers()
        {
            var playerOneName = "player1";
            var playerTwoName = "player2";
            var invalidPlayerName = "player3";

            var validPlayers = new string[2];
            validPlayers[0] = playerOneName;
            validPlayers[1] = playerTwoName;

            var game = new Game(gameId, 4, 4, validPlayers, winningPieces);

            Assert.IsTrue(game.IsValidPlayer(playerOneName));
            Assert.IsTrue(game.IsValidPlayer(playerTwoName));
            Assert.IsFalse(game.IsValidPlayer(invalidPlayerName));
        }

        [Test]
        public void FullBoardNoWinnerIsADraw()
        {
            var game = new Game(gameId, 2, 2, players, winningPieces);
            game.PerformMove(new TokenMove(players[0], 0));
            game.PerformMove(new TokenMove(players[1], 1));
            game.PerformMove(new TokenMove(players[0], 1));
            game.PerformMove(new TokenMove(players[1], 0));

            Assert.AreEqual(Game.GameStates.DONE, game.GameState);
            Assert.IsNull(game.Winner);
        }
    }
}
