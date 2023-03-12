using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public class ReplyToken
    {
        // Properties
        public string UserId { get; set; } = string.Empty;

        public string TokenValue { get; set; } = string.Empty;  

        public DateTime ExpiredTime { get; set; }       
    }
}
