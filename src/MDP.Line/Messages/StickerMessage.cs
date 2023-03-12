using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public class StickerMessage : Message
    {
        // Constants
        private const string DefaultMessageType = "Sticker";


        // Constructors
        public StickerMessage() : base()
        {
            // Default
            this.MessageType = StickerMessage.DefaultMessageType;
        }

        public StickerMessage(Message message) : base(message)
        {
            // Default
            this.MessageType = StickerMessage.DefaultMessageType;
        }


        // Properties
        public int PackageId
        {
            get { return this.GetIntContent("PackageId"); }
            set { this.SetContent("PackageId", value); }
        }

        public int StickerId
        {
            get { return this.GetIntContent("StickerId"); }
            set { this.SetContent("StickerId", value); }
        }


        // Methods
        public static StickerMessage? ToStickerMessage(Message message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Require
            if (message.MessageType != StickerMessage.DefaultMessageType) return null;
            if (message is StickerMessage) return (StickerMessage?)message;

            // Return
            return new StickerMessage(message);
        }
    }
}
