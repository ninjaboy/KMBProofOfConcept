using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            await next.Invoke(context);
        }
    }
}
