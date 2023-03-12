using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;

namespace MDP.Line.WebApp
{
    public partial class HomeController : Controller
    {
        // Fields                
        private readonly LineContext _lineContext;


        // Constructors
        public HomeController(LineContext lineContext)
        {
            #region Contracts

            if (lineContext == null) throw new ArgumentException($"{nameof(lineContext)}=null");

            #endregion

            // Default
            _lineContext = lineContext;
        }


        // Methods
        public ActionResult Index()
        {
            return View();
        }
    }

    public partial class HomeController : Controller
    {
        // Methods
        public ActionResult<FindAllUserResultModel> FindAllUser([FromBody] FindAllUserActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // 測試用：要認證帳號or企業帳號才能抓取用戶清單，所以先用收到訊息去重讀用戶清單
            {
                // FindAllMessage
                var messageList = _lineContext.MessageRepository.FindAll();
                if (messageList == null) throw new InvalidOperationException($"{nameof(messageList)}=null");
                foreach (var message in messageList ) 
                {
                    // ReloadUser
                    if (_lineContext.UserRepository.FindByUserId(message.UserId) == null)
                    {
                        
                        _lineContext.ReloadUser(message.UserId);
                    }
                }
            }

            // FindAllUser
            var userList = _lineContext.UserRepository.FindAll();
            if (userList == null) throw new InvalidOperationException($"{nameof(userList)}=null");

            // Return
            return (new FindAllUserResultModel()
            {
                UserList = userList
            });
        }


        // Class
        public class FindAllUserActionModel
        {
            // Properties

        }

        public class FindAllUserResultModel
        {
            // Properties
            public List<User>? UserList { get; set; }
        }
    }

    public partial class HomeController : Controller
    {
        // Methods
        public ActionResult<FindAllMessageResultModel> FindAllMessage([FromBody] FindAllMessageActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // FindAllMessage
            var userList = _lineContext.MessageRepository.FindAllByUserId(actionModel.UserId);
            if (userList == null) throw new InvalidOperationException($"{nameof(userList)}=null");

            // Return
            return (new FindAllMessageResultModel()
            {
                MessageList = userList
            });
        }


        // Class
        public class FindAllMessageActionModel
        {
            // Properties
            public string UserId { get; set; } = string.Empty;
        }

        public class FindAllMessageResultModel
        {
            // Properties
            public List<Message>? MessageList { get; set; }
        }
    }

    public partial class HomeController : Controller
    {
        // Methods
        public ActionResult<SendTextMessageResultModel> SendTextMessage([FromBody] SendTextMessageActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // TextMessage
            var textMessage = new TextMessage();
            textMessage.MessageId = Guid.NewGuid().ToString();
            textMessage.UserId = actionModel.UserId;
            textMessage.SenderId = actionModel.SenderId;
            textMessage.Text = actionModel.Text;
            textMessage.CreatedTime = DateTime.Now;

            // SendMessage
            _lineContext.SendMessage(textMessage);

            // Return
            return (new SendTextMessageResultModel()
            {

            });
        }


        // Class
        public class SendTextMessageActionModel
        {
            // Properties
            public string UserId { get; set; } = string.Empty;

            public string SenderId { get; set; } = string.Empty;

            public string Text { get; set; } = string.Empty;
        }

        public class SendTextMessageResultModel
        {
            // Properties
            
        }
    }

    public partial class HomeController : Controller
    {
        // Methods
        public ActionResult<SendStickerMessageResultModel> SendStickerMessage([FromBody] SendStickerMessageActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // StickerMessage
            var textMessage = new StickerMessage();
            textMessage.MessageId = Guid.NewGuid().ToString();
            textMessage.UserId = actionModel.UserId;
            textMessage.SenderId = actionModel.SenderId;
            textMessage.PackageId = actionModel.PackageId;
            textMessage.StickerId = actionModel.StickerId;
            textMessage.CreatedTime = DateTime.Now;

            // SendMessage
            _lineContext.SendMessage(textMessage);

            // Return
            return (new SendStickerMessageResultModel()
            {

            });
        }


        // Class
        public class SendStickerMessageActionModel
        {
            // Properties
            public string UserId { get; set; } = string.Empty;

            public string SenderId { get; set; } = string.Empty;

            public int PackageId { get; set; } = 0;

            public int StickerId { get; set; } = 0;
        }

        public class SendStickerMessageResultModel
        {
            // Properties

        }
    }
}
