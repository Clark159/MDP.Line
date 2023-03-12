using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public interface MessageRepository
    {
        // Methods
        void Add(Message message);

        List<Message> FindAll();

        List<Message> FindAllByUserId(string userId);
    }
}
