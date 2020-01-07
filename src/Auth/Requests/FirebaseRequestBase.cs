using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Firebase.Auth.Requests
{
    public class IdTokenRequest
    {
        public string IdToken { get; set; }
    }

    /// <summary>
    /// Base class for issuing http requests against google <see cref="Endpoints"/>.
    /// </summary>
    /// <typeparam name="TRequest"> Specifies the type of request payload. </typeparam>
    /// <typeparam name="TResponse"> Specifies the type of response payload. </typeparam>
    public abstract class FirebaseRequestBase<TRequest, TResponse>
    {
        protected readonly FirebaseAuthConfig config;

        protected FirebaseRequestBase(FirebaseAuthConfig config)
        {
            this.config = config;
        }

        protected abstract string UrlFormat { get; }

        protected virtual HttpMethod Method => HttpMethod.Post;

        protected virtual JsonSerializerSettings JsonSettingsOverride => null;

        public virtual async Task<TResponse> ExecuteAsync(TRequest request)
        {
            var responseData = string.Empty;
            var requestData = request != null ? JsonConvert.SerializeObject(request, this.JsonSettingsOverride ?? this.config.JsonSettings) : null;
            var url = this.GetFormattedUrl(this.config.ApiKey);

            try
            {
                var content = request != null ? new StringContent(requestData, Encoding.UTF8, "application/json") : null;
                var message = new HttpRequestMessage(this.Method, url)
                {
                    Content = content
                };

                var httpResponse = await this.config.HttpClient.SendAsync(message).ConfigureAwait(false);
                responseData = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                var response = JsonConvert.DeserializeObject<TResponse>(responseData, this.JsonSettingsOverride ?? this.config.JsonSettings);

                httpResponse.EnsureSuccessStatusCode();

                return response;
            }
            catch (Exception ex)
            {
                var errorReason = FirebaseFailureParser.GetFailureReason(responseData);
                throw new FirebaseAuthHttpException(ex, url, requestData, responseData, errorReason);
            }
        }

        private string GetFormattedUrl(string apiKey)
        {
            return string.Format(this.UrlFormat, apiKey);
        }
    }
}
