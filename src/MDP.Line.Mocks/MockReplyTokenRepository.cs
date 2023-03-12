using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Mocks;

namespace MDP.Line.Mocks
{
    public class MockReplyTokenRepository : ReplyTokenRepository
    {
        // Fields
        private readonly object _syncRoot = new object();

        private readonly Dictionary<string, Queue<ReplyToken>> _replyTokenQueueList = new Dictionary<string, Queue<ReplyToken>>();


        // Methods
        public void Enqueue(ReplyToken replyToken)
        {
            #region Contracts

            if (replyToken == null) throw new ArgumentException($"{nameof(replyToken)}=null");

            #endregion

            // ReplyTokenQueue
            var replyTokenQueue = this.CreateReplyTokenQueue(replyToken.UserId);
            if (replyTokenQueue == null) throw new InvalidOperationException ($"{nameof(replyTokenQueue)}=null");

            // Sync            
            lock (_syncRoot)
            {
                // Enqueue
                replyTokenQueue.Enqueue(replyToken);
            }
        }

        public ReplyToken? Dequeue(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // ReplyTokenQueue
            var replyTokenQueue = this.CreateReplyTokenQueue(userId);
            if (replyTokenQueue == null) throw new InvalidOperationException($"{nameof(replyTokenQueue)}=null");

            // Sync            
            lock (_syncRoot)
            {
                // Require
                if (replyTokenQueue.Count == 0) return null;

                // Enqueue
                return replyTokenQueue.Dequeue();
            }
        }

        private Queue<ReplyToken> CreateReplyTokenQueue(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion
            // Sync            
            lock (_syncRoot)
            {
                // ReplyTokenQueue
                Queue<ReplyToken>? replyTokenQueue = null;
                if (_replyTokenQueueList.ContainsKey(userId) == false)
                {
                    // Create
                    replyTokenQueue = new Queue<ReplyToken>();

                    // Add
                    _replyTokenQueueList.Add(userId, replyTokenQueue);
                }
                else
                {
                    // Get
                    replyTokenQueue = _replyTokenQueueList[userId];
                }
                if (replyTokenQueue == null) throw new InvalidOperationException($"{nameof(replyTokenQueue)}=null");

                // Return
                return replyTokenQueue;
            }
        }
    }
}