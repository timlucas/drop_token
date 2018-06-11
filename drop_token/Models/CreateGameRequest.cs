using System;
using System.Collections.Generic;

namespace drop_token.Models
{
    public class CreateGameRequest
    {
        public int columns { get; set; }
        public int rows { get; set; }
        public List<String> players { get; set; }
    }
}