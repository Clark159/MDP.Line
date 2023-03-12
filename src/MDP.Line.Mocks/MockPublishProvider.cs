using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Mocks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MDP.Line.Mocks
{
    public class MockPublishProvider : PublishProvider
    {
        // Methods
        public void OnUserFollowed(User user)
        {
            #region Contracts

            if (user == null) throw new ArgumentException($"{nameof(user)}=null");

            #endregion

            // Display
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} UserFollowed:\n User={JsonSerializer.Serialize(user)}\n");
        }

        public void OnUserUnfollowed(User user)
        {
            #region Contracts

            if (user == null) throw new ArgumentException($"{nameof(user)}=null");

            #endregion

            // Display
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} UserUnfollowed:\n User={JsonSerializer.Serialize(user)}\n");
        }

        public void OnMessageReceived(Message message)
        {
            #region Contracts

            if (message == null) throw new ArgumentException($"{nameof(message)}=null");

            #endregion

            // Display
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} MessageReceived:\n Message={JsonSerializer.Serialize(message)}\n");
        }
    }
}