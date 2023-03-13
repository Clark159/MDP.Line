using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Line
{
    public class User
    {
        // Constructors
        public User()
        {
            // Default
           
        }

        public User(User user)
        {
            #region Contracts

            if (user == null) throw new ArgumentException($"{nameof(user)}=null");

            #endregion

            // Default
            this.UserId = user.UserId;
            this.Name = user.Name;
            this.Mail = user.Mail;
            this.Phone = user.Phone;
            this.PictureUrl = user.PictureUrl;
            this.IsFollowed = user.IsFollowed;
            this.UpdatedTime = user.UpdatedTime;
            this.CreatedTime = user.CreatedTime;
        }


        // Properties
        public string UserId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Mail { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PictureUrl { get; set; } = string.Empty;

        public bool IsFollowed { get; set; } = true;

        public DateTime UpdatedTime { get; set; } = DateTime.Now;

        public DateTime CreatedTime { get; set; } = DateTime.Now;


        // Methods
        public static User CreateUser(User user)
        {
            #region Contracts

            if (user == null) throw new ArgumentException($"{nameof(user)}=null");

            #endregion

            // ResultUser
            var resultUser = new User(user);
            {

            }

            // Return
            return resultUser;
        }

        public static User CreateUser(User user, User minorUser)
        {
            #region Contracts

            if (user == null) throw new ArgumentException($"{nameof(user)}=null");
            if (minorUser == null) throw new ArgumentException($"{nameof(minorUser)}=null");

            #endregion

            // ResultUser
            var resultUser = new User(user);
            {
                resultUser.UserId = string.IsNullOrEmpty(resultUser.UserId) == false ? resultUser.UserId : minorUser.UserId;
                resultUser.Name = string.IsNullOrEmpty(resultUser.Name) == false ? resultUser.Name : minorUser.Name;
                resultUser.Mail = string.IsNullOrEmpty(resultUser.Mail) == false ? resultUser.Mail : minorUser.Mail;
                resultUser.Phone = string.IsNullOrEmpty(resultUser.Phone) == false ? resultUser.Phone : minorUser.Phone;
                resultUser.PictureUrl = string.IsNullOrEmpty(resultUser.PictureUrl) == false ? resultUser.PictureUrl : minorUser.PictureUrl;
                resultUser.IsFollowed = resultUser.IsFollowed;
                resultUser.UpdatedTime = resultUser.UpdatedTime >= minorUser.UpdatedTime ? resultUser.UpdatedTime : minorUser.UpdatedTime;
                resultUser.CreatedTime = resultUser.CreatedTime <= minorUser.CreatedTime ? resultUser.CreatedTime : minorUser.CreatedTime;
            }

            // Return
            return resultUser;
        }
    }
}
