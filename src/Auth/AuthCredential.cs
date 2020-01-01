using System.Collections.Generic;
using System.Linq;

namespace Firebase.Auth
{
    public class AuthCredential
    {
        public FirebaseProviderType ProviderType { get; set; }

        public object Object { get; set; }

        public T CopyToSetAccountRequest<T>(T request)
            where T: IDictionary<string, object>
        {
            // copy values of all properties from Object to request
            foreach (var property in this.Object.GetType().GetProperties())
            {
                var name = property.Name.First().ToString().ToLower() + property.Name.Substring(1);
                request[name] = property.GetValue(this.Object);
            }

            return request;
        }
    }
}
