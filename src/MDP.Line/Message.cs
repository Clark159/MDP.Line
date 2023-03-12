using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public class Message
    {
        // Constructors
        public Message()
        {
            // Default
            this.Contents = new Dictionary<string, string> ();
        }

        public Message(Message message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Default
            this.MessageId = message.MessageId;
            this.MessageType = message.MessageType;
            this.UserId = message.UserId;
            this.SenderId = message.SenderId;
            this.Contents = message.Contents;
            this.CreatedTime = message.CreatedTime;
        }


        // Properties
        public string MessageId { get; set; } = string.Empty;

        public string MessageType { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;

        public bool IsFromUser { get { return (this.UserId == this.SenderId); } }

        public Dictionary<string, string> Contents { get; }

        public DateTime CreatedTime { get; set; }


        // Methods
        protected int GetIntContent(string name)
        {
            // Result
            int valueResult = 0;

            // Require
            if (this.Contents.ContainsKey(name) == false) return valueResult;

            // ValueString
            var valueString = this.Contents[name];
            if (string.IsNullOrEmpty(valueString) == true) return valueResult;

            // Parse
            if (int.TryParse(valueString, out var value) == true)
            {
                valueResult = value;
            }

            // Return
            return valueResult;
        }

        protected string GetStringContent(string name)
        {
            // Result
            string valueResult = string.Empty;

            // Require
            if (this.Contents.ContainsKey(name) == false) return valueResult;

            // ValueString
            var valueString = this.Contents[name];
            if (string.IsNullOrEmpty(valueString) == true) return valueResult;

            // Parse
            valueResult = valueString;

            // Return
            return valueResult;
        }

        protected void SetContent(string name, object value)
        {
            // ValueString
            var valueString = string.Empty;
            if (value != null) { valueString = value.ToString(); }

            // Set
            this.Contents[name] = valueString!;
        }
    }
}
