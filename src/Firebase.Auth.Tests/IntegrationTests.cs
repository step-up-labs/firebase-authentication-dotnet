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

        private const string FirebaseEmail = "<TEST USER EMAIL>";
        private const string FirebasePassword = "<TEST USER PASSWORD>";

        [TestMethod]
        public void FacebookTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            var auth = authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, FacebookAccessToken).Result;

            auth.User.FirstName.ShouldBeEquivalentTo(FacebookTestUserFirstName);
            auth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void GoogleTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            var auth = authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google, GoogleAccessToken).Result;

            auth.User.FirstName.ShouldBeEquivalentTo(GoogleTestUserFirstName);
            auth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void EmailTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            var auth = authProvider.SignInWithEmailAndPasswordAsync(FirebaseEmail, FirebasePassword).Result;

            auth.User.Email.ShouldBeEquivalentTo(FirebaseEmail);
            auth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void CreateUserTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var email = $"abcd{new Random().Next()}@test.com"; 

            var auth = authProvider.SignInWithEmailAndPasswordAsync(email, "test1234").Result;

            auth.User.Email.ShouldBeEquivalentTo(email);
            auth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void LinkAccountsTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var email = $"abcd{new Random().Next()}@test.com";

            var auth = authProvider.SignInAnonymouslyAsync().Result;
            var newAuth = auth.LinkToAsync(email, "test1234").Result;

            newAuth.User.Email.ShouldBeEquivalentTo(email);
            newAuth.User.LocalId.Should().Be(auth.User.LocalId);
            newAuth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void LinkAccountsFacebookTest()
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            var auth = authProvider.SignInAnonymouslyAsync().Result;
            var newAuth = auth.LinkToAsync(FirebaseAuthType.Facebook, FacebookAccessToken).Result;

            newAuth.User.LocalId.Should().Be(auth.User.LocalId);
            newAuth.User.FirstName.ShouldBeEquivalentTo(FacebookTestUserFirstName);
            newAuth.FirebaseToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}
