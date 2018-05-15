using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace KMBProofOfConcept
{
    public class RandomMiddleware
    {
        private readonly RequestDelegate next;

        public RandomMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        private async Task<RequestInputModel> GetRequestInputs(HttpContext context)
        {
            context.Request.EnableRewind();
            using (var reader = new StreamReader(context.Request.Body))
            {
                return new RequestInputModel
                {
                    QueryString = context.Request.QueryString.Value,
                    RequestBody = await reader.ReadToEndAsync()
                };
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestInputModel = await GetRequestInputs(context);
            var inputModel = JsonConvert.SerializeObject(requestInputModel);
            Console.WriteLine(inputModel);

            await next.Invoke(context);

            var output = await GetResponseOutput(context);
            Console.WriteLine(output);
        }

        private async Task<string> GetResponseOutput(HttpContext context)
        {
            using (var buffer = new MemoryStream())
            {
                var originalBodyStream = context.Response.Body;
                context.Response.Body = buffer;

                await next.Invoke(context);

                buffer.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(buffer).ReadToEndAsync();

                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;

                return responseBody;
            }
        }

        private class RequestInputModel
        {
            public string QueryString { get; set; }
            public string RequestBody { get; set; }
        }
    }
}
