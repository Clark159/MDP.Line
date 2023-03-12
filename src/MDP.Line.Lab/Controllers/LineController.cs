using isRock.LineBot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MDP.Line.Lab.Controllers
{    
    public class LineController : Controller
    {
        // Fields
        private readonly string _channelAccessToken = "3ifdMSUeLzs8q5e0yQLrpUy8uQgG0kEXiOju2fr79lfc2t7Q9Ui6CCTvaJyuadf2ntYeY1GdZqHWr6s6bDE1kek5jbbEGUVEHAuEI3f/sPn9OyEjUJuQNNPmdaJhhsTyaU77iGnXfO8HPm4oN8tbrAdB04t89/1O/w1cDnyilFU=";
        
        private readonly string _channelSecret = "37bebf7ccef77b5e630ffcba9d0ccb50";
                
        private readonly Bot _lineSdk;


        // Constructors
        public  LineController()
        {
            // Default
            _lineSdk = new Bot(_channelAccessToken);
        }


        [HttpPost]
        [Route("/Hook-Line", Name = "Hook-Line")]
        public async Task< ActionResult> Hook()
        {
            try
            {
                // Content
                var content = string.Empty;
                using (var reader = new StreamReader(this.Request.Body))
                {
                    content = await reader.ReadToEndAsync();
                }
                if (string.IsNullOrEmpty(content) == true) return this.BadRequest();
                if (this.ValidateSignature(content, this.Request.Headers) == false) return this.BadRequest();

                // ReceivedMessage
                var receivedMessage = isRock.LineBot.Utility.Parsing(content);
                if (receivedMessage == null) throw new InvalidOperationException($"{nameof(receivedMessage)}=null");

                // Event
                foreach(var receivedEvent in receivedMessage.events)
                {
                    // Variables
                    var userId = receivedEvent.source.userId;
                    var messageType = receivedEvent.message?.type ?? receivedEvent.type;
                    var messageId = receivedEvent.message?.id;
                    var replyToken = receivedEvent.replyToken;

                    // Message
                    var message = "";
                    message += $"UserId: {userId}\n";
                    message += $"MessageType: {messageType}\n";
                    message += $"MessageId: {messageId}\n";
                    switch (messageType)
                    {
                        case "text":
                            {
                                // ReplyMessage
                                message += $"Text: {receivedEvent.message!.text}\n";
                                isRock.LineBot.Utility.ReplyMessage(replyToken, message, _channelAccessToken);
                            }
                            break;

                        case "sticker":
                            {
                                // ReplyMessage
                                Exception? exception = null;
                                try
                                {
                                    isRock.LineBot.Utility.ReplyStickerMessage(replyToken, receivedEvent.message!.packageId, receivedEvent.message!.stickerId, _channelAccessToken);
                                }
                                catch (Exception ex)
                                {
                                    exception = ex;
                                }

                                // PushMessage
                                if (exception != null)
                                {
                                    message += $"PackageId: {receivedEvent.message!.packageId}\n";
                                    message += $"StickerId: {receivedEvent.message!.stickerId}\n";
                                    message += $"Error: {exception.Message}\n";
                                    _lineSdk.PushMessage(userId, message);
                                }
                            }
                            break;

                        default:
                            {
                                // ReplyMessage
                                isRock.LineBot.Utility.ReplyMessage(replyToken, message, _channelAccessToken);
                            }
                            break;
                    }
                }
                
                // PushMessage
                //lineSdk.PushMessage(userId, "Test"); // 文字
                //lineSdk.PushMessage(userId, 1, 2); // 貼圖
                //lineSdk.PushMessage(userId, new Uri(@"https://s.yimg.com/cv/apiv2/twfrontpage/logo/Yahoo-TW-desktop-FP@2x.png"));
            }
            catch (Exception ex)
            {
                // TODO

            }
            
            // Return
            return this.Ok();
        }

        private bool ValidateSignature(string content, IDictionary<string, StringValues> headerList)
        {
            // Require
            if (string.IsNullOrEmpty(content) == true) return false;
            if (headerList == null) return false;

            // Signature 
            var signature = string.Empty;
            if (headerList.TryGetValue("X-Line-Signature", out var signatureHeader) == true)
            {
                signature = signatureHeader.FirstOrDefault();
            }
            if (string.IsNullOrEmpty(signature) == true) return false;

            // ValidateSignature
            return this.ValidateSignature(content, signature);
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
    }
}
