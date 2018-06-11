using DropTokenGame;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace drop_token.Models
{
    public class GameStatusResponse
    {
        public List<String> players { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string winner { get; set; }

        public string state { get; set; }

        public GameStatusResponse()
        {
        }

        public GameStatusResponse(GameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException("gameState", "A valid game state must be specified.");
            }

            this.players = gameState.Players;
            this.state = gameState.State;
            this.winner = gameState.Winner;
        }
    }
}