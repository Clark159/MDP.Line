using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public class LineContext
    {
        // Fields
        private readonly UserRepository _userRepository;

        private readonly MessageRepository _messageRepository;

        private readonly ReplyTokenRepository _replyTokenRepository;

        private readonly LineProvider _lineProvider;

        private readonly PublishProvider _publishProvider;


        // Constructors
        public LineContext
        (
            UserRepository userRepository,
            MessageRepository messageRepository,
            ReplyTokenRepository replyTokenRepository,
            LineProvider lineProvider,
            PublishProvider publishProvider
        )
        {
            #region Contracts

            if (userRepository == null) throw new ArgumentException($"{nameof(userRepository)}=null");
            if (messageRepository == null) throw new ArgumentException($"{nameof(messageRepository)}=null");
            if (replyTokenRepository == null) throw new ArgumentException($"{nameof(replyTokenRepository)}=null");
            if (lineProvider == null) throw new ArgumentException($"{nameof(lineProvider)}=null");
            if (publishProvider == null) throw new ArgumentException($"{nameof(publishProvider)}=null");

            #endregion

            // Default
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _replyTokenRepository = replyTokenRepository;
            _lineProvider = lineProvider;
            _publishProvider = publishProvider;

            // LineProvider
            _lineProvider.UserFollowed += this.LineProvider_UserFollowed;
            _lineProvider.UserUnfollowed += this.LineProvider_UserUnfollowed;
            _lineProvider.MessageReceived += this.LineProvider_MessageReceived;
        }


        // Properties
        public UserRepository UserRepository { get { return _userRepository; } }

        public MessageRepository MessageRepository { get { return _messageRepository; } }


        // Methods
        public void HandleHook(string content, string signature)
        {
            #region Contracts

            if (string.IsNullOrEmpty(content) == true) throw new ArgumentException($"{nameof(content)}=null");
            if (string.IsNullOrEmpty(signature) == true) throw new ArgumentException($"{nameof(signature)}=null");

            #endregion

            // HandleHook
            _lineProvider.HandleHook(content, signature);
        }


        public void SendMessage(TextMessage message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // SendMessage
            this.SendMessage(message, _lineProvider.SendMessage);
        }

        public void SendMessage(StickerMessage message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // SendMessage
            this.SendMessage(message, _lineProvider.SendMessage);
        }

        private void SendMessage<TMessage>(TMessage message, Action<TMessage, ReplyToken?> sendMessage) where TMessage : Message
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");
            if (sendMessage == null) throw new ArgumentException($"{nameof(sendMessage)}=null");

            #endregion

            // Require
            if (message.IsFromUser == true) throw new InvalidOperationException("message.IsFromUser=true");

            // ReplyMessage
            while (true)
            {
                // ReplyToken
                var replyToken = _replyTokenRepository.Dequeue(message.UserId);
                if (replyToken == null) break;

                // Execute
                try
                {
                    // SendMessage
                    sendMessage(message, replyToken);

                    // Add
                    _messageRepository.Add(message);

                    // Publish
                    this.OnMessageReceived(message);
                    _publishProvider.OnMessageReceived(message);

                    // Return
                    return;
                }
                catch { }
            }

            // PushMessage
            {
                // SendMessage
                sendMessage(message, null);

                // Add
                _messageRepository.Add(message);

                // Publish
                this.OnMessageReceived(message);
                _publishProvider.OnMessageReceived(message);
            }
        }


        public User? ReloadUser(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // LocalUser
            var localUser = _userRepository.FindByUserId(userId);

            // RemoteUser
            var remoteUser = _lineProvider.FindUserByUserId(userId);
            if (remoteUser == null && localUser == null) return null;
            if (remoteUser == null && localUser != null) return localUser;

            // ResultUser
            User resultUser;
            if (localUser == null)
            {
                // Add
                resultUser = User.CreateUser(remoteUser!);
                {
                    resultUser.IsFollowed = true;
                    resultUser.UpdatedTime = DateTime.Now;
                }
                _userRepository.Add(resultUser);
            }
            else
            {
                // Update
                resultUser = User.CreateUser(localUser, remoteUser!);
                {
                    resultUser.IsFollowed = true;
                    resultUser.UpdatedTime = DateTime.Now;
                }
                _userRepository.Update(resultUser);
            }

            // Return
            return resultUser;
        }


        // Handlers
        private void LineProvider_UserFollowed(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // LocalUser
            var localUser = _userRepository.FindByUserId(userId);

            // RemoteUser
            var remoteUser = _lineProvider.FindUserByUserId(userId);
            if (remoteUser == null) throw new InvalidOperationException($"{nameof(remoteUser)}=null");

            // ResultUser
            User resultUser;
            if (localUser == null)
            {
                // Add
                resultUser = User.CreateUser(remoteUser);
                {
                    resultUser.IsFollowed = true;
                    resultUser.UpdatedTime = DateTime.Now;
                }
                _userRepository.Add(resultUser);
            }
            else
            {
                // Update
                resultUser = User.CreateUser(localUser, remoteUser);
                {
                    resultUser.IsFollowed = true;
                    resultUser.UpdatedTime = DateTime.Now;
                }
                _userRepository.Update(resultUser);
            }

            // Publish
            this.OnUserFollowed(resultUser);
            _publishProvider.OnUserFollowed(resultUser);
        }

        private void LineProvider_UserUnfollowed(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // LocalUser
            var localUser = _userRepository.FindByUserId(userId);
            if (localUser == null) return;

            // ResultUser
            User resultUser = User.CreateUser(localUser);
            {
                // Update
                {
                    resultUser.IsFollowed = false;
                    resultUser.UpdatedTime = DateTime.Now;
                }
                _userRepository.Update(resultUser);
            }

            // Publish
            this.OnUserUnfollowed(resultUser);
            _publishProvider.OnUserUnfollowed(resultUser);
        }

        private void LineProvider_MessageReceived(Message message, ReplyToken replyToken)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");
            if (replyToken == null) throw new ArgumentException($"{nameof(replyToken)}=null");

            #endregion

            // Add
            _messageRepository.Add(message);
            _replyTokenRepository.Enqueue(replyToken);

            // Publish
            this.OnMessageReceived(message);
            _publishProvider.OnMessageReceived(message);
        }


        // Events
        public event Action<User>? UserFollowed;
        private void OnUserFollowed(User user)
        {
            #region Contracts

            if (user == null) throw new ArgumentException($"{nameof(user)}=null");

            #endregion

            // Raise
            var handler = this.UserFollowed;
            if (handler != null)
            {
                handler(user);
            }
        }

        public event Action<User>? UserUnfollowed;
        private void OnUserUnfollowed(User user)
        {
            #region Contracts

            if (user == null) throw new ArgumentException($"{nameof(user)}=null");

            #endregion

            // Raise
            var handler = this.UserUnfollowed;
            if (handler != null)
            {
                handler(user);
            }
        }

        public event Action<Message>? MessageReceived;
        private void OnMessageReceived(Message message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Raise
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(message);
            }
        }
    }
}
