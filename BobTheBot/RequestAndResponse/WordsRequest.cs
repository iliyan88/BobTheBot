using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheBot.RequestAndResponse
{
    public class WordsRequest
    {
        public IEnumerable<string> Words { get; set; }
    }
}
