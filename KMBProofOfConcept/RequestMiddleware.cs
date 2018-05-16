using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

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
                using (var buffer = new MemoryStream())
                {
                    context.Request.EnableRewind(); // !<<<
                    await context.Request.Body.CopyToAsync(buffer); // |-|

                    buffer.Seek(0, SeekOrigin.Begin); // <-
                    context.Request.Body.Seek(0, SeekOrigin.Begin);  // <-

                    var input = await new StreamReader(buffer).ReadToEndAsync(); // o-o
                    Console.WriteLine($"=> Request Body; '{input}'"); // < )))

                    await next.Invoke(context); // >>>
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
