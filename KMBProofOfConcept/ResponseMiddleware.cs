using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KMBProofOfConcept
{
    public class ResponseMiddleware : BaseStreamMiddleware
    {
        private Stream original;

        public ResponseMiddleware(RequestDelegate next) : base(next) { }

        protected override async Task Before(HttpContext context, MemoryStream buffer)
        {
            original = context.Response.Body; // <->
            context.Response.Body = buffer; // <->
        }

        protected override async Task After(HttpContext context, MemoryStream buffer)
        {
            buffer.Seek(0, SeekOrigin.Begin); // <-
            var output = await new StreamReader(buffer).ReadToEndAsync(); // o-o
            Console.WriteLine($"=> Response Body: '{output}'"); // < )))

            buffer.Seek(0, SeekOrigin.Begin);// <-
            await buffer.CopyToAsync(original); // |-|
            context.Response.Body = original; // <->
        }
    }
}
