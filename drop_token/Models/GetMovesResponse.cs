using System;
using System.Collections.Generic;

namespace drop_token.Models
{
    public class GetMovesResponse
    {
        public List<GetMoveResponse> moves { get; set; }
    }
}