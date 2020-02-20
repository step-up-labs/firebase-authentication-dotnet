using System;

namespace Firebase.Auth.Tests
{
    public class Setup 
    {
        public const string AuthDomainEnvironmentVariableName = "FIREBASE_AUTH_TEST_DOMAIN";
        public const string ApiKeyEnvironmentVariableName = "FIREBASE_AUTH_TEST_API_KEY";

        public static Setup LoadFromEnvironment()
        {
            return new Setup
            {
                ApiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariableName),
                AuthDomain = Environment.GetEnvironmentVariable(AuthDomainEnvironmentVariableName)
            };
        }

        public string AuthDomain { get; private set; }

        public string ApiKey { get; private set; }
    }
}
