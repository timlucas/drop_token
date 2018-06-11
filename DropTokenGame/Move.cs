using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropTokenGame
{
    public abstract class Move
    {
        public string Player { get; protected set; }
        public string MoveType { get; protected set; }

        public abstract bool IsValidForPlayer(string player);
        public abstract int Apply(Game game);
    }

    public static class MoveTypes
    {
        public static string MOVE = "MOVE";
        public static string QUIT = "QUIT";
    }
}
