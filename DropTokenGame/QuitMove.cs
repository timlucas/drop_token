using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame
{
    public class QuitMove : Move
    {
        public QuitMove(string player)
        {
            this.Player = player;
            this.MoveType = MoveTypes.QUIT;
        }

        public override bool IsValidForPlayer(string player)
        {
            // any player may quit at any time
            return true;
        }

        public override int Apply(Game game)
        {
            return game.QuitGame(this);
        }
    }
}
