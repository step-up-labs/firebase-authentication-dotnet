using System;
using Xunit;

namespace Firebase.Auth.Tests
{
    public class ClientTests
    {
        [Fact]
        public void MissingAPIKeyTest()
        {
            Assert.Throws<ArgumentException>(() => new FirebaseAuthClient(new FirebaseAuthConfig { AuthDomain = "testdomain" }));
        }

        [Fact]
        public void MissingAuthRedirectKeyTest()
        {
            Assert.Throws<ArgumentException>(() => new FirebaseAuthClient(new FirebaseAuthConfig { ApiKey = "TestApiKey" }));
        }
    }
}
