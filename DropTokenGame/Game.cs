using DropTokenGame.GameExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame
{
    public class Game
    {
        public string GameId { get; private set; }
        public string[] Players { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public int WinningPiecesInARow { get; private set; }
        public List<Move> Moves { get; private set; }
        public string GameState { get; private set; }
        public string Winner { get; private set; }

        private int currentPlayerIndex;
        private char[,] gameBoard;
        private int[] lowestEmptyRows;        // will hold the current spots for each column where token should drop; will start at 0
                                              // sacrifice some space to keep time down, especially if we start using boards much larger than 4x4
        private int fullColumns;              // keep track of how many full columns we have; when == Columns, the board is full...we don't need to check every column every time
        private int moveCount;
        private int maxMoves;

        public Game(string id, int columns, int rows, string[] players, int winningPieces) {
            this.GameId = id;
            this.Columns = columns;
            this.Rows = rows;
            this.WinningPiecesInARow = winningPieces;

            initializePlayers(players);
            initializeGameState();
            initializeBoard();
        }

        private void initializePlayers(string[] players)
        {
            if (players.Length != 2)
            {
                throw new ArgumentException("Games must have exactly 2 players");
            }

            this.Players = new string[2];
            int i = 0;
            foreach (string player in players)
            {
                if (String.IsNullOrWhiteSpace(player))
                {
                    throw new ArgumentException(String.Format("Player {0} must have non-empty name", i));
                }
                this.Players[i] = player;
                i++;
            }
        }

        private void initializeGameState()
        {
            this.currentPlayerIndex = 0;
            this.GameState = GameStates.IN_PROGRESS;
            this.Moves = new List<Move>();
            this.moveCount = 0;
            this.maxMoves = (Rows * Columns) + 1;   // we can have at most one move per spot on the board, plus a final quit if one of the players chooses to do so
        }

        private void initializeBoard()
        {
            this.gameBoard = new char[Rows, Columns];
            this.lowestEmptyRows = new int[Columns];
            this.fullColumns = 0;
        }

        private bool isInProgress()
        {
            return this.GameState == GameStates.IN_PROGRESS;
        }

        public bool IsValidPlayer(string player)
        {
            return (player == Players[0] || player == Players[1]);
        }

        /// <summary>
        /// Return the other player in the game
        /// </summary>
        /// <param name="player">The player we're looking at</param>
        private string getOtherPlayer(string player)
        {
            return (Players[0] == player) ? Players[1] : Players[0];
        }

        private string CurrentPlayer()
        {
            return Players[currentPlayerIndex];
        }

        private string changeNextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex == 0) ? 1 : 0;
            return Players[currentPlayerIndex];
        }

        private char currentPlayerPiece()
        {
            return playerPiece(Players[currentPlayerIndex]);
        }

        private char playerPiece(string player)
        {
            if (player == Players[0])
                return '1';

            return '2';
        }

        /// <summary>
        /// Perform a move in the game (dropping a token or quitting)
        /// </summary>
        /// <param name="move">The move to be performed</param>
        /// <returns>The moveNumber of the move (if valid); 0-based</returns>
        public int PerformMove(Move move)
        {
            if (!IsValidPlayer(move.Player))
                throw new InvalidPlayerException(string.Format("Player {0} is not playing game {1}", move.Player, GameId));

            if (!isInProgress())
                throw new IllegalMoveException(String.Format("Game {0} is already done; player {1} cannot quit", GameId, move.Player));

            if (!move.IsValidForPlayer(CurrentPlayer()))
                throw new OutOfTurnException(String.Format("Game {0}: move made out of turn by player {1}", GameId, move.Player));

            return move.Apply(this);   // change the game/gamestate as appropriate for the move
        }
        
        internal int QuitGame(QuitMove move)
        {
            var thisMoveNumber = moveCount;

            if (moveCount < maxMoves)
            {
                Moves.Add(move);
                moveCount++;
            }
            else
            {
                // should never get here - if we've had more moves than spaces, the dropToken method should already have thrown for a full column
                throw new IllegalMoveException("More moves than there are spaces!");
            }
            
            // a player can quit a game at every moment while the game is in progress
            this.GameState = GameStates.DONE;
            Winner = getOtherPlayer(move.Player);

            return thisMoveNumber;
        }

        internal int PlaceToken(TokenMove move)
        {
            if (move.Column >= Columns)
            {
                throw new IllegalMoveException(string.Format("Column {0} does not exist!", move.Column));
            }

            var tokenRow = dropToken(move.Player, move.Column);     // will throw exception if column is full; let bubble up

            var thisMoveNumber = moveCount;

            if (moveCount < maxMoves)
            {
                Moves.Add(move);
                moveCount++;
            }
            else
            {
                // should never get here - if we've had more moves than spaces, the dropToken method should already have thrown for a full column
                throw new IllegalMoveException("More moves than there are spaces!");
            }

            if (moveCount >= ((WinningPiecesInARow * 2) - 1))
            {
                // no point in checking for a win until enough tokens have been dropped to even allow a win
                if (isWinningMove(move.Player, tokenRow, move.Column))
                {
                    GameState = GameStates.DONE;
                    Winner = move.Player;
                }
            }
            else if (moveCount == (Rows * Columns))
            {
                if (boardIsFull())      // redundant, really...we've just had as many successful moves as spaces in the board
                {
                    GameState = GameStates.DONE;
                    Winner = null;          // should already be null; just being explicit about it
                }
            }

            changeNextPlayer();

            return thisMoveNumber;
        }

        /// <summary>
        /// Try to drop the token in the specified column
        /// </summary>
        /// <returns>The row level at which the token landed</returns>
        private int dropToken(string player, int column)
        {
            var piece = playerPiece(player);
            var rowForThisPiece = lowestEmptyRows[column];

            // if the column already has [Rows] number of tokens, it's full
            if (rowForThisPiece == Rows)
            {
                throw new IllegalMoveException(String.Format("Column {0} in game {1} is full", column, GameId));
            }

            gameBoard[rowForThisPiece, column] = piece;
            lowestEmptyRows[column] = rowForThisPiece + 1;
            if (lowestEmptyRows[column] == Rows)        // if the column is now full
            {
                fullColumns += 1;
            }

            return rowForThisPiece;
        }

        /// <summary>
        /// Checks to see if we have enough tokens in a row for the current move to have won the game for its player
        /// </summary>
        /// <param name="player">The player who made the most recent move</param>
        /// <param name="row">The row of the most recent move</param>
        /// <param name="column">The column of the most recent move</param>
        private bool isWinningMove(string player, int row, int column)
        {
            var stretchSpaces = WinningPiecesInARow - 1;
            var rangeLengthToCheck = (2 * stretchSpaces) + 1;   // if WinningPiecesInARow = 4, we check the current spot (O) 
                                                                // and enough to either side (X's) to know if there's a win: | |X|X|X|O|X|X|X| |

            // first check the row
            var isWin = checkRange(row, (column - stretchSpaces), 0, 1, rangeLengthToCheck);
            if (!isWin)
            {
                // now check the column
                isWin = checkRange((row - stretchSpaces), column, 1, 0, rangeLengthToCheck);
            }
            if (!isWin)
            {
                // now check the minor diagonal
                isWin = checkRange((row - stretchSpaces), (column - stretchSpaces), 1, 1, rangeLengthToCheck);
            }
            if (!isWin)
            {
                // now check the major diagonal
                isWin = checkRange((row + stretchSpaces), (column - stretchSpaces), -1, 1, rangeLengthToCheck);
            }

            return isWin;
        }

        private bool boardIsFull()
        {
            return fullColumns == Columns;
        }

        private bool checkRange(int startRow, int startColumn, int rowIncrement, int columnIncrement, int steps)
        {
            var currentPiece = currentPlayerPiece();
            var contiguousPieceCount = 0;

            var row = startRow;
            var col = startColumn;

            for (int x = 0; x < steps; x++)
            {
                if (isValidBoardPosition(row, col))
                {
                    if (gameBoard[row, col] == currentPiece)
                    {
                        contiguousPieceCount += 1;
                        if (contiguousPieceCount == WinningPiecesInARow)
                            return true;    // short circuit - we know it's a win if we have enough winning pieces
                    }
                    else
                    {
                        contiguousPieceCount = 0;       // wrong piece; reset and start over counting until end of range
                    }
                }

                row += rowIncrement;
                col += columnIncrement;
            }

            return false;
        }

        private bool isValidBoardPosition(int row, int column)
        {
            return (row >= 0 && row < Rows && column >= 0 && column < Columns);
        }

        public static class GameStates
        {
            public static string IN_PROGRESS = "IN_PROGRESS";
            public static string DONE = "DONE";
        }
    }
}
