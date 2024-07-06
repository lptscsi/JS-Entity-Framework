using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RIAppDemo.Utils
{
    public static class BodyReadHelper
    {
        public static async Task<string> GetRawBodyAsync(
            this HttpRequest request,
            Encoding encoding = null)
        {
            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
