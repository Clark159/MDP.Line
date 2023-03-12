using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line.Mocks
{
    public class MockLineProvider : LineProvider
    {
        // Methods
        public override void HandleHook(string content, string signature)
        {
            #region Contracts

            if (string.IsNullOrEmpty(content) == true) throw new ArgumentException($"{nameof(content)}=null");
            if (string.IsNullOrEmpty(signature) == true) throw new ArgumentException($"{nameof(signature)}=null");

            #endregion

            // Message
            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                MessageType = "text",
                UserId = "U89f2966454791597236a20f676e989dc",
                SenderId = "U89f2966454791597236a20f676e989dc",
                CreatedTime = DateTime.Now,
            };
            message.Contents.Add("text", "123456789");

            // ReplyToken
            var replyToken = new ReplyToken()
            {
                UserId = "U89f2966454791597236a20f676e989dc",
                TokenValue = "12345",
                ExpiredTime = DateTime.Now.AddSeconds(10)
            };

            // OnMessageReceived
            this.OnMessageReceived(message, replyToken);
        }

        public override void PushMessage(Message message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Nothing

        }

        public override void ReplyMessage(Message message, ReplyToken replyToken)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");
            if (replyToken == null) throw new ArgumentException($"{nameof(replyToken)}=null");

            #endregion

            // Nothing

        }

        public override User? FindUserByUserId(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // Return
            return new User()
            {
                UserId = userId,
                Name = "Clark",
                IsFollowed = true
            };
        }
    }
}
