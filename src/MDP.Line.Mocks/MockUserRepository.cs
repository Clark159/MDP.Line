using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLK.Mocks;

namespace MDP.Line.Mocks
{
    public class MockUserRepository : MockRepository<User, string>, UserRepository
    {
        // Constructors
        public MockUserRepository() : base(user => Tuple.Create(user.UserId))
        {
            // Default

        }


        // Methods
        public User? FindByUserId(string userId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(userId) == true) throw new ArgumentException($"{nameof(userId)}=null");

            #endregion

            // FindById
            return this.FindById(userId);
        }
    }
}