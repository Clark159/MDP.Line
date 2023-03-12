using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public interface PublishProvider
    {
        // Methods
        void OnUserFollowed(User user);

        void OnUserUnfollowed(User user);

        void OnMessageReceived(Message message);
    }
}
