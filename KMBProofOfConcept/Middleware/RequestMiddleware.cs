using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace KMBProofOfConcept.Middleware
{
    public class RequestMiddleware : BaseStreamMiddleware
    {
        public RequestMiddleware(RequestDelegate next) : base(next) { }

        protected override async Task Before(HttpContext context, MemoryStream buffer)
        {
            context.Request.EnableRewind(); // !<<<
            await context.Request.Body.CopyToAsync(buffer); // |-|

            buffer.Seek(0, SeekOrigin.Begin); // <-
            context.Request.Body.Seek(0, SeekOrigin.Begin);  // <-

            var input = await new StreamReader(buffer).ReadToEndAsync(); // o-o
            Console.WriteLine($"=> Request Body; '{input}'"); // < )))
        }
    }
}
