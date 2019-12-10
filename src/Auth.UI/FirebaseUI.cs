using System;

namespace Firebase.Auth.UI
{
    public class FirebaseUI
    {
        private static FirebaseUI firebaseUI;
        private readonly FirebaseAuthClient client;

        private FirebaseUI(FirebaseAuthConfigUI config)
        {
            this.client = new FirebaseAuthClient(config);
        }

        public static FirebaseUI Initialize(FirebaseAuthConfigUI config)
        {
            if (firebaseUI != null)
            {
                throw new InvalidOperationException("FirebaseUI has already been initialized");
            }

            return firebaseUI = new FirebaseUI(config);
        }

        public static FirebaseUI Instance => firebaseUI ?? throw new InvalidOperationException("FirebaseUI hasn't been initialized yet.");
    }
}
