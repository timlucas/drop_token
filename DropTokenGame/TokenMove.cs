using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame
{
    public class TokenMove : Move
    {
        public int Column { get; private set; }

        public TokenMove(string player, int column)
        {
            this.Player = player;
            this.Column = column;
            this.MoveType = MoveTypes.MOVE;
        }

        public override bool IsValidForPlayer(string player)
        {
            return player == this.Player;
        }

        public override int Apply(Game game)
        {
            return game.PlaceToken(this);
        }
    }
}
