namespace Firebase.Auth.Tests
{
    using System;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class IntegrationTests
    {
        private const string ApiKey = "<YOUR API KEY>";

        private const string FacebookAccessToken = "<FACEBOOK USER ACCESS TOKEN>";
        private const string FacebookTestUserFirstName = "Mark";

        private const string GoogleAccessToken = "<GOOGLE USER ACCESS TOKEN>";
        private const string GoogleTestUserFirstName = "Mark";

        [TestMethod]
        public void FacebookTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            var auth = authProvider.SignInWithOAuth(FirebaseAuthType.Facebook, FacebookAccessToken).Result;

            auth.User.FirstName.ShouldBeEquivalentTo(FacebookTestUserFirstName);
            auth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void GoogleTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            var auth = authProvider.SignInWithOAuth(FirebaseAuthType.Google, GoogleAccessToken).Result;

            auth.User.FirstName.ShouldBeEquivalentTo(GoogleTestUserFirstName);
            auth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}
