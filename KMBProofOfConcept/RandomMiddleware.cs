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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Read Request Body
                context.Request.EnableRewind();
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var requestInputModel = new RequestInputModel
                    {
                        QueryString = context.Request.QueryString.Value,
                        RequestBody = await reader.ReadToEndAsync()
                    };
                    var inputModel = JsonConvert.SerializeObject(requestInputModel);
                    Console.WriteLine(inputModel);
                }

                // Read Response Body
                using (var buffer = new MemoryStream())
                {
                    var original = context.Response.Body;
                    context.Response.Body = buffer;

                    // Invoke Next Middleware
                    await next.Invoke(context);

                    buffer.Seek(0, SeekOrigin.Begin);
                    var output = await new StreamReader(buffer).ReadToEndAsync();
                    buffer.Seek(0, SeekOrigin.Begin);

                    await buffer.CopyToAsync(original);
                    context.Response.Body = original;

                    Console.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private class RequestInputModel
        {
            public string QueryString { get; set; }
            public string RequestBody { get; set; }
        }
    }
}
