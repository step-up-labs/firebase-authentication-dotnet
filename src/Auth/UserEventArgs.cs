using System;

namespace Firebase.Auth
{
    public class UserEventArgs : EventArgs
    {
        public UserEventArgs(User user)
        {
            this.User = user;
        }

        /// <summary>
        /// Currently signed in user. Null if no user is signed in.
        /// </summary>
        public User User { get; }
    }
}
