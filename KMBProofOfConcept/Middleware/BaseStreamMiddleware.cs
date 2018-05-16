using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KMBProofOfConcept.Middleware
{
    public abstract class BaseStreamMiddleware
    {
        private readonly RequestDelegate next;

        protected BaseStreamMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        protected virtual async Task Before(HttpContext context, MemoryStream buffer) {;}

        protected virtual async Task After(HttpContext context, MemoryStream buffer) {;}

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                using (var buffer = new MemoryStream())
                {
                    await Before(context, buffer);
                    await next.Invoke(context);
                    await After(context, buffer);
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
