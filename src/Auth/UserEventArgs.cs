using System;

namespace Firebase.Auth
{
    public class UserEventArgs : EventArgs
    {
        public UserEventArgs(User user)
        {
            this.User = user;
        }

        public User User { get; }
    }
}
