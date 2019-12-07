using System.Net.Http;

namespace Firebase.Auth.Requests
{
    internal class ProjectConfigResponse
    {
        public string ProjectId { get; set; }

        public string[] AuthorizedDomains { get; set; }
    }

    internal class ProjectConfig : FirebaseRequestBase<object, ProjectConfigResponse>
    {
        public ProjectConfig(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleProjectConfighUrl;

        protected override HttpMethod Method => HttpMethod.Get;
    }
}
