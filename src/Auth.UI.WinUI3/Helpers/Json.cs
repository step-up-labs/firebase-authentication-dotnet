using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Firebase.Auth.UI.Helpers
{
    public static class Json
    {
        public static async Task<T> ToObjectAsync<T>(string value)
        {
            return await Task.Run<T>(() =>
            {
                return JsonConvert.DeserializeObject<T>(value);
            }).ConfigureAwait(false);
        }

        public static async Task<string> StringifyAsync(object value)
        {
            return await Task.Run<string>(() =>
            {
                return JsonConvert.SerializeObject(value);
            }).ConfigureAwait(false);
        }
    }

}

