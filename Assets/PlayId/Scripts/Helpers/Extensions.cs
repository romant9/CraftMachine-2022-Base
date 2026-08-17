using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.PlayId.Scripts.Helpers
{
    public static class Extensions
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<AsyncOperation>();

            asyncOp.completed += operation => { tcs.SetResult(operation); };

            return ((Task) tcs.Task).GetAwaiter();
        }

        public static string GetError(this UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success) return null;

            var error = request.error;

            if (error == "Cannot resolve destination host" || error == "Cannot connect to destination host") return $"{error}: {request.uri}";

            if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
            {
                if (request.downloadHandler.text.Contains("\"error\""))
                {
                    error = JObject.Parse(request.downloadHandler.text)["error"].Value<string>();
                }
                else
                {
                    error = request.downloadHandler.text;
                }
            }

            if (!error.EndsWith('.')) error += '.';

            return error;
        }
    }
}