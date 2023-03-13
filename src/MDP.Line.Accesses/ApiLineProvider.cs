using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace MDP.Line.Accesses
{
    public class ApiLineProvider : LineProvider
    {
        // Fields
        private readonly string _channelAccessToken = string.Empty;

        private readonly string _channelSecret = string.Empty;

        private readonly isRock.LineBot.Bot _lineSdk;


        // Constructors
        public ApiLineProvider(string channelAccessToken, string channelSecret)
        {
            #region Contracts

            if (string.IsNullOrEmpty(channelAccessToken) == true) throw new ArgumentException($"{nameof(channelAccessToken)}=null");
            if (string.IsNullOrEmpty(channelSecret) == true) throw new ArgumentException($"{nameof(channelSecret)}=null");

            #endregion

            // Default
            _channelAccessToken = channelAccessToken;
            _channelSecret = channelSecret;
            _lineSdk = new Bot(_channelAccessToken);
        }


        // Methods
        public override void HandleHook(string content, string signature)
        {
            #region Contracts

            if (string.IsNullOrEmpty(content) == true) throw new ArgumentException($"{nameof(content)}=null");
            if (string.IsNullOrEmpty(signature) == true) throw new ArgumentException($"{nameof(signature)}=null");

            #endregion

            // Require
            if (this.ValidateSignature(content, signature) == false) throw new InvalidOperationException($"ValidateSignature failed");

            // LineMessage
            var lineMessage = isRock.LineBot.Utility.Parsing(content);
            if (lineMessage == null) throw new InvalidOperationException($"{nameof(lineMessage)}=null");

            // LineEvent
            var exceptionList = new List<Exception>();
            foreach (var lineEvent in lineMessage.events)
            {
                try
                {
                    // HandleEvent
                    this.HandleEvent(lineEvent);
                }
                catch (Exception ex)
                {
                    // Add
                    exceptionList.Add(ex);
                }
            }
            if (exceptionList.Count > 0) throw new AggregateException(exceptionList);
        }

        private void HandleEvent(isRock.LineBot.Event lineEvent)
        {
            #region Contracts

            if (lineEvent == null) throw new ArgumentException($"{nameof(lineEvent)}=null");

            #endregion

            // HandleEvent
            switch (lineEvent.type.ToLower())
            {
                case "follow":
                    this.HandleFollowEvent(lineEvent);
                    break;

                case "unfollow":
                    this.HandleUnfollowEvent(lineEvent);
                    break;

                case "message":
                    this.HandleMessageEvent(lineEvent);
                    break;

                default:
                    this.HandleUnknownEvent(lineEvent);
                    break;
            }
        }

        private void HandleFollowEvent(isRock.LineBot.Event lineEvent)
        {
            #region Contracts

            if (lineEvent == null) throw new ArgumentException($"{nameof(lineEvent)}=null");

            #endregion

            // UserId
            var userId = lineEvent.source?.userId ?? string.Empty;
            if (string.IsNullOrEmpty(userId) == true) return;

            // OnUserFollowed
            this.OnUserFollowed(userId);
        }

        private void HandleUnfollowEvent(isRock.LineBot.Event lineEvent)
        {
            #region Contracts

            if (lineEvent == null) throw new ArgumentException($"{nameof(lineEvent)}=null");

            #endregion

            // UserId
            var userId = lineEvent.source?.userId ?? string.Empty;
            if (string.IsNullOrEmpty(userId) == true) return;

            // OnUserUnfollowed
            this.OnUserUnfollowed(userId);
        }

        private void HandleMessageEvent(isRock.LineBot.Event lineEvent)
        {
            #region Contracts

            if (lineEvent == null) throw new ArgumentException($"{nameof(lineEvent)}=null");

            #endregion

            // ReplyToken
            var replyToken = new ReplyToken();
            replyToken.UserId = lineEvent.source?.userId ?? string.Empty;
            replyToken.TokenValue = lineEvent.replyToken ?? string.Empty;
            replyToken.ExpiredTime = DateTime.Now.AddSeconds(9);

            // Message
            Message? message = null;
            switch (lineEvent.message?.type?.ToLower())
            {
                case "text":
                    message = new TextMessage()
                    {
                        Text = lineEvent.message?.text ?? string.Empty
                    };
                    break;

                case "sticker":
                    message = new StickerMessage()
                    {
                        PackageId = lineEvent.message?.packageId ?? 0,
                        StickerId = lineEvent.message?.stickerId ?? 0,
                    };
                    break;

                default:
                    this.HandleUnknownEvent(lineEvent);
                    return;
            }
            if (message != null)
            {
                message.MessageId = lineEvent.message?.id ?? string.Empty;
                message.UserId = lineEvent.source?.userId ?? string.Empty;
                message.SenderId = lineEvent.source?.userId ?? string.Empty;
                message.CreatedTime = DateTime.Now;
            }
            if (message == null) throw new InvalidOperationException($"{nameof(message)}=null");

            // OnMessageReceived
            this.OnMessageReceived(message, replyToken);
        }

        private void HandleUnknownEvent(isRock.LineBot.Event lineEvent)
        {
            #region Contracts

            if (lineEvent == null) throw new ArgumentException($"{nameof(lineEvent)}=null");

            #endregion

            // ReplyToken
            var replyToken = lineEvent.replyToken;

            // Message
            var message = "";
            message += $"EventType: {lineEvent.type}\n";
            message += $"MessageType: {lineEvent.message?.type}\n";
            message += $"MessageId: {lineEvent.message?.id}\n";
            message += $"UserId: {lineEvent.source.userId}\n";

            // ReplyMessage
            isRock.LineBot.Utility.ReplyMessage(replyToken, message, _channelAccessToken);
        }

        private bool ValidateSignature(string content, string signature)
        {
            // Require
            if (string.IsNullOrEmpty(content) == true) return false;
            if (string.IsNullOrEmpty(signature) == true) return false;

            // Validate
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            byte[] channelSecretBytes = Encoding.UTF8.GetBytes(_channelSecret);
            if (Convert.ToBase64String(new HMACSHA256(channelSecretBytes).ComputeHash(contentBytes)) != signature)
            {
                return false;
            }

            // Return
            return true;
        }


        public override void SendMessage(TextMessage message, ReplyToken? replyToken = null)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Variables
            var text = message.Text.Replace("'", "\"");

            // MessageObject
            dynamic messageObject = new ExpandoObject();
            {
                // Message
                messageObject.type = "text";
                messageObject.text = text;

                // Sender
                messageObject.sender = new ExpandoObject();
                messageObject.sender.name = "Clark";
                messageObject.sender.iconUrl = @"https://sprofile.line-scdn.net/0hwTIGPma7KHtkPj653TZWBBRuKxFHT3FpQQpgSQQ-fh9fCD0sSlg1HwNrdEpRWz0rTwtgG1ZpdBhoLV8demjUT2MOdkxdCWguS1xjnQ";
            }

            // SendMessage            
            this.SendMessage(new List<dynamic>() { messageObject }, message.UserId, replyToken?.TokenValue);
        }

        public override void SendMessage(StickerMessage message, ReplyToken? replyToken = null)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Variables
            var packageId = message.PackageId;
            var stickerId = message.StickerId;

            // MessageObject
            dynamic messageObject = new ExpandoObject();
            {
                // Message
                messageObject.type = "sticker";
                messageObject.packageId = packageId;
                messageObject.stickerId = stickerId;

                // Sender
                messageObject.sender = new ExpandoObject();
                messageObject.sender.name = "Clark";
                messageObject.sender.iconUrl = @"https://sprofile.line-scdn.net/0hwTIGPma7KHtkPj653TZWBBRuKxFHT3FpQQpgSQQ-fh9fCD0sSlg1HwNrdEpRWz0rTwtgG1ZpdBhoLV8demjUT2MOdkxdCWguS1xjnQ";
            }

            // SendMessage            
            this.SendMessage(new List<dynamic>() { messageObject }, message.UserId, replyToken?.TokenValue);
        }

        private string SendMessage(dynamic messageList, string userId, string? replyToken = null)
        {
            #region Contracts

            if (messageList == null) throw new ArgumentException($"{nameof(messageList)}=null");
            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // CommandUrl
            var commandUrl = string.Empty;
            if (replyToken == null)
            {
                commandUrl = @"https://api.line.me/v2/bot/message/push";
            }
            else
            {
                commandUrl = @"https://api.line.me/v2/bot/message/reply";
            }

            // CommandString
            dynamic command = new ExpandoObject();
            {
                // Destination 
                if (string.IsNullOrEmpty(replyToken) == true)
                {
                    command.to = userId;
                }
                else
                {
                    command.replyToken = replyToken;
                }

                // Message
                command.messages = messageList;
            }
            var commandString = JsonSerializer.Serialize(command);
            if (string.IsNullOrEmpty(commandString) == true) throw new InvalidOperationException($"{nameof(commandString)}=null");

            // SendCommand
            return this.SendCommand(commandUrl, commandString);
        }


        public override User? FindUserByUserId(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // RemoteUser:錯誤處理還沒做
            var remoteUser = _lineSdk.GetUserInfo(userId);
            if (remoteUser == null) return null;

            // Return
            return new User()
            {
                UserId = userId,
                Name = remoteUser.displayName,
                Mail = string.Empty,
                Phone = string.Empty,
                PictureUrl = remoteUser.pictureUrl,
                IsFollowed = true,
                UpdatedTime = DateTime.Now,
                CreatedTime = DateTime.Now,
            };
        }


        private string SendCommand(string commandUrl, string commandString)
        {
            #region Contracts

            if (string.IsNullOrEmpty(commandUrl) == true) throw new ArgumentException($"{nameof(commandUrl)}=null");
            if (string.IsNullOrEmpty(commandString) == true) throw new ArgumentException($"{nameof(commandString)}=null");

            #endregion

            // SendMessage            
            try
            {
                // WebClient
                using (var webClient = new WebClient())
                {
                    // Headers
                    webClient.Headers.Clear();
                    webClient.Headers.Add("Content-Type", "application/json");
                    webClient.Headers.Add("Authorization", "Bearer " + _channelAccessToken);

                    // Send
                    var commandContentBytes = Encoding.UTF8.GetBytes(commandString);
                    var commandResultBytes = webClient.UploadData(commandUrl, commandContentBytes);
                    if (commandResultBytes == null) throw new InvalidOperationException($"{nameof(commandResultBytes)}=null");

                    // CommandResult
                    var commandResult = Encoding.UTF8.GetString(commandResultBytes);
                    if (string.IsNullOrEmpty(commandResult) == true) throw new InvalidOperationException($"{nameof(commandResult)}=null");

                    // Return
                    return commandResult;
                }
            }
            catch (WebException ex)
            {
                // CommandResult
                var commandResult = string.Empty;
                using (var streamReader = new StreamReader(ex.Response!.GetResponseStream()))
                {
                    commandResult = streamReader.ReadToEnd();
                }
                if (string.IsNullOrEmpty(commandResult) == true) throw new InvalidOperationException($"{nameof(commandResult)}=null");

                // Throw
                throw new Exception($"LineAPI Error: CommandUrl={commandUrl}, commandString={commandString}, commandResult={commandResult}", ex);
            }
        }
    }
}