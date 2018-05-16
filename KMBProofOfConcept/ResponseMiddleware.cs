using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KMBProofOfConcept
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate next;

        public ResponseMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
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
    }
}
