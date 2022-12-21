namespace Firebase.Auth
{
    /// <summary>
    /// The auth config. 
    /// </summary>
    public class FirebaseConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseConfig"/> class.
        /// </summary>
        /// <param name="apiKey"> The api key of your Firebase app. </param>
        /// <param name="usesEmulator"> If the emulator is being used. Default is false. </param>
        /// <param name="emulatorPort"> What port the emulator is using. Default is 9099. </param>
        public FirebaseConfig(string apiKey, bool usesEmulator = false, int emulatorPort = 9099)
        {
            this.ApiKey = apiKey;
            this.UsesEmulator = usesEmulator;
            this.EmulatorPort = emulatorPort;
        }

        /// <summary>
        /// Gets or sets the api key of your Firebase app. 
        /// </summary>
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the emulator is used or not. 
        /// </summary>
        public bool UsesEmulator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port the emulator uses. 
        /// </summary>
        public int EmulatorPort
        {
            get;
            set;
        }
    }
}
