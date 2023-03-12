using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public class TextMessage : Message
    {
        // Constants
        private const string DefaultMessageType = "Text";


        // Constructors
        public TextMessage() : base() 
        {
            // Default
            this.MessageType = TextMessage.DefaultMessageType;
        }

        public TextMessage(Message message) : base(message)
        {
            // Default
            this.MessageType = TextMessage.DefaultMessageType;
        }


        // Properties
        public string Text
        {
            get { return this.GetStringContent("Text"); }
            set { this.SetContent("Text", value); }
        }


        // Methods
        public static TextMessage? ToTextMessage(Message message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Require
            if (message.MessageType != TextMessage.DefaultMessageType) return null;
            if (message is TextMessage) return (TextMessage?)message;

            // Return
            return new TextMessage(message);
        }
    }
}
