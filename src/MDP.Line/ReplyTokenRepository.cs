using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public interface ReplyTokenRepository
    {
        // Methods
        void Enqueue(ReplyToken replyToken);

        ReplyToken? Dequeue(string userId);
    }
}
