using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame
{
    public class GameState
    {
        public List<string> Players { get; private set; }
        public string Winner { get; private set; }
        public string State { get; private set; }

        public GameState(Game game)
        {
            this.Players = new List<string>();

            this.Players.Add(game.Players[0]);
            this.Players.Add(game.Players[1]);
            this.State = game.GameState;

            if (game.GameState == Game.GameStates.DONE)
                Winner = game.Winner;
        }
    }
}
