using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Firebase.Auth.Tests
{
    public class ErrorParserTests
    {
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] {
                    @"{
                        ""error"": {
                        ""code"": 400,
                        ""message"": ""WEAK_PASSWORD : Password should be at least 6 characters"",
                        ""errors"": [
                            {
                            ""message"": ""WEAK_PASSWORD : Password should be at least 6 characters"",
                            ""domain"": ""global"",
                            ""reason"": ""invalid""
                            }
                        ]
                        }
                    }", 
                    AuthErrorReason.WeakPassword },
                new object[] {
                    @"{
                        ""error"": {
                        ""code"": 400,
                        ""message"": ""INVALID_PASSWORD"",
                        ""errors"": [
                            {
                            ""message"": ""INVALID_PASSWORD"",
                            ""domain"": ""global"",
                            ""reason"": ""invalid""
                            }
                        ]
                        }
                    }",
                    AuthErrorReason.WrongPassword },
                new object[] {
                    @"{
                      ""error"": {
                        ""code"": 400,
                        ""message"": ""INVALID_IDP_RESPONSE : Bad access token: {\""code\"":190,\""subcode\"":463,\""message\"":\""Error validating access token: Session has expired on Friday, 03-Jan-20 15:00:00 PST. The current time is Wednesday, 08-Jan-20 06:04:25 PST.\""}"",
                        ""errors"": [
                          {
                            ""message"": ""INVALID_IDP_RESPONSE : Bad access token: {\""code\"":190,\""subcode\"":463,\""message\"":\""Error validating access token: Session has expired on Friday, 03-Jan-20 15:00:00 PST. The current time is Wednesday, 08-Jan-20 06:04:25 PST.\""}"",
                            ""domain"": ""global"",
                            ""reason"": ""invalid""
                          }
                        ]
                      }
                    }",
                    AuthErrorReason.InvalidAccessToken},

            };

        [Theory]
        [MemberData(nameof(Data))]
        public void Test(string response, AuthErrorReason error)
        {
            var result = FirebaseFailureParser.GetFailureReason(response);

            result.Should().Be(error);
        }
    }
}
