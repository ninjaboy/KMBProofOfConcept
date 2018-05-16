using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace KMBProofOfConcept
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate next;

        public RequestMiddleware(RequestDelegate next)
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

                // Invoke Next Middleware
                await next.Invoke(context);
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
