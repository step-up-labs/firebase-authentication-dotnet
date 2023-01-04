using System.Net.Http;

namespace Firebase.Auth.Requests
{
    public class ProjectConfigResponse
    {
        public string ProjectId { get; set; }

        public string[] AuthorizedDomains { get; set; }
    }

    /// <summary>
    /// Get basic config info about the firebase project.
    /// </summary>
    public class ProjectConfig : FirebaseRequestBase<object, ProjectConfigResponse>
    {
        public ProjectConfig(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleProjectConfighUrl;

        protected override HttpMethod Method => HttpMethod.Get;
    }
}
