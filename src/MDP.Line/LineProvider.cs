using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public abstract class LineProvider
    {
        // Methods
        public abstract void HandleHook(string content, string signature);

        public abstract void SendMessage(TextMessage message, ReplyToken? replyToken = null);

        public abstract void SendMessage(StickerMessage message, ReplyToken? replyToken = null);

        public abstract User? FindUserByUserId(string userId);


        // Events
        public event Action<string>? UserFollowed;
        protected void OnUserFollowed(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // Raise
            var handler = this.UserFollowed;
            if (handler != null)
            {
                handler(userId);
            }
        }

        public event Action<string>? UserUnfollowed;
        protected void OnUserUnfollowed(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // Raise
            var handler = this.UserUnfollowed;
            if (handler != null)
            {
                handler(userId);
            }
        }

        public event Action<Message, ReplyToken>? MessageReceived;
        protected void OnMessageReceived(Message message, ReplyToken replyToken)
        {
            #region Contracts

            if (message==null) throw new ArgumentException($"{nameof(message)}=null");
            if (replyToken == null) throw new ArgumentException($"{nameof(replyToken)}=null");

            #endregion

            // Raise
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(message, replyToken);
            }
        }
    }
}
