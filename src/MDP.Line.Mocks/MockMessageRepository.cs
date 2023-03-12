using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Mocks;

namespace MDP.Line.Mocks
{
    public class MockMessageRepository : MockRepository<Message, string>, MessageRepository
    {
        // Constructors
        public MockMessageRepository() : base(message => Tuple.Create(message.MessageId))
        {
            // Default

        }


        // Methods
        public List<Message> FindAllByUserId(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // FindAll
            return this.EntityList.Where(x => x.UserId == userId).OrderBy(x => x.CreatedTime).ToList();
        }
    }
}