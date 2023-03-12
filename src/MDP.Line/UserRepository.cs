using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public interface UserRepository
    {
        // Methods
        void Add(User user);

        void Update(User user);

        User? FindByUserId(string userId);

        List<User> FindAll();
    }
}
