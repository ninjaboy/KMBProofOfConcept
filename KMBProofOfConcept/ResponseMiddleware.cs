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
                using (var buffer = new MemoryStream())
                {
                    var original = context.Response.Body; // <->
                    context.Response.Body = buffer; // <->

                    await next.Invoke(context); // >>>

                    buffer.Seek(0, SeekOrigin.Begin); // <-
                    var output = await new StreamReader(buffer).ReadToEndAsync(); // o-o
                    Console.WriteLine($"=> Response Body: '{output}'"); // < )))

                    buffer.Seek(0, SeekOrigin.Begin);// <-
                    await buffer.CopyToAsync(original); // |-|
                    context.Response.Body = original; // <->
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
