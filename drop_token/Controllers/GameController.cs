using drop_token.Models;
using DropTokenGame;
using DropTokenGame.GameExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace drop_token.Controllers
{
    [RoutePrefix("drop_token")]
    public class GameController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetGames()
        {
            var games = GameKeeper.GetGameIds();

            var response = new GetGamesResponse() { games = games };

            return Ok(response);
        }

        [HttpPost]
        [Route("")]
        public CreateGameResponse CreateGame(CreateGameRequest request)
        {
            try
            {
                var gameId = GameKeeper.AddGame(request.rows, request.columns, request.players);
                return new CreateGameResponse() { gameId = gameId };
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) 
                                                                        { Content = new StringContent(ex.Message) });
            }
        }

        [HttpGet]
        [Route("{gameId}")]
        public GameStatusResponse GetGameStatus(string gameId)
        {
            var gameStatus = GameKeeper.GetGameState(gameId);
            var response = new GameStatusResponse(gameStatus);

            return response;
        }

        [HttpGet]
        [Route("{gameId}/moves")]
        public GetMovesResponse GetGameMoves(string gameId, [FromUri]int? start = null, [FromUri]int? until = null)
        {
            try
            {
                var moves = GameKeeper.GetGameMoves(gameId, start, until);
                var moveResults = new List<GetMoveResponse>();

                foreach(var move in moves)
                {
                    var moveResult = new GetMoveResponse()
                    {
                        player = move.Player,
                        type = move.MoveType
                    };
                    if (move is TokenMove)
                    {
                        moveResult.column = ((TokenMove)move).Column;
                    };
                    moveResults.Add(moveResult);
                }

                return new GetMovesResponse() { moves = moveResults };
            }
            catch (KeyNotFoundException ex)
            {
                // game not found
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (ApplicationException ex)
            {
                // moves not found
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (IndexOutOfRangeException ex)
            {
                // invalid start or end indexes = malformed request
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(ex.Message) });
            }
            catch (ArgumentException ex)
            {
                // the end index is less than the start index - malformed request
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(ex.Message) });
            }
        }

        [HttpPost]
        [Route("{gameId}/{playerId}")]
        public PostMoveResponse PostMove(string gameId, string playerId, [FromBody]PostMoveRequest request)
        {
            try
            {
                var move = new TokenMove(playerId, request.column);
                var moveNumber = GameKeeper.RegisterMove(gameId, move);

                return new PostMoveResponse() { move = String.Format("{0}/moves/{1}", gameId, moveNumber) };
            }
            catch (KeyNotFoundException ex)
            {
                // game not found
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (InvalidPlayerException ex)
            {
                // player not part of the game
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (IllegalMoveException ex)
            {
                // game is done, or column is full
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(ex.Message) });
            }
            catch (OutOfTurnException ex)
            {
                // not the player's turn
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(ex.Message) });
            }
        }

        [HttpGet]
        [Route("{gameId}/moves/{move_number}")]
        public GetMoveResponse GetMove(string gameId, int move_number)
        {
            try
            {
                var move = GameKeeper.GetGameMove(gameId, move_number);

                var response = new GetMoveResponse()
                {
                    type = move.MoveType,
                    player = move.Player
                };

                if (move.MoveType == MoveTypes.MOVE)
                {
                    response.column = ((TokenMove)move).Column;
                }

                return response;
            }
            catch (KeyNotFoundException ex)
            {
                // game not found
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (ApplicationException ex)
            {
                // game moves not found
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // invalid move number
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
        }

        [HttpDelete]
        [Route("{gameId}/{playerId}")]
        public HttpResponseMessage QuitGame(string gameId, string playerId)
        {
            try
            {
                var move = new QuitMove(playerId);
                var moveNum = GameKeeper.RegisterMove(gameId, move);

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (KeyNotFoundException ex)
            {
                // game not found
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (InvalidPlayerException ex)
            {
                // player is not part of game
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(ex.Message) });
            }
            catch (IllegalMoveException ex)
            {
                // game is already done
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Gone) { Content = new StringContent(ex.Message) });
            }
        }
    }
}
