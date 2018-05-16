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
                // Read Response Body
                using (var buffer = new MemoryStream())
                {
                    context.Request.EnableRewind();

                    var original = context.Request.Body;

                    await original.CopyToAsync(buffer);
                    buffer.Seek(0, SeekOrigin.Begin);
                    original.Seek(0, SeekOrigin.Begin);

                    var output = await new StreamReader(original).ReadToEndAsync();

                    // Invoke Next Middleware
                    await next.Invoke(context);

                    //buffer.Seek(0, SeekOrigin.Begin);
                    //await buffer.CopyToAsync(original);
                    //context.Request.Body = original;

                    Console.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    try
        //    {
        //        using (var reader = new StreamReader(context.Request.Body))
        //        {
        //            context.Request.EnableRewind();
        //            var requestInputModel = new RequestInputModel
        //            {
        //                QueryString = context.Request.QueryString.Value,
        //                RequestBody = await reader.ReadToEndAsync()
        //            };
        //            var inputModel = JsonConvert.SerializeObject(requestInputModel);
        //            Console.WriteLine(inputModel);
        //            context.Request.Body.Seek(0, SeekOrigin.Begin);

        //            // Invoke Next Middleware
        //            await next.Invoke(context);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        throw;
        //    }
        //}

        private class RequestInputModel
        {
            public string QueryString { get; set; }
            public string RequestBody { get; set; }
        }
    }
}
