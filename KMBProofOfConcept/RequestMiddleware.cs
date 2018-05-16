namespace KMBProofOfConcept
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.Logging;

    public class RequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestMiddleware> logger;

        public RequestMiddleware(RequestDelegate next, ILogger<RequestMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            var requestBodyStream = new MemoryStream();
            var originalRequestBody = context.Request.Body;

            await context.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            var url = context.Request.GetDisplayUrl();
            var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
            logger.LogInformation($"REQUEST METHOD: {context.Request.Method}, REQUEST BODY: {requestBodyText}, REQUEST URL: {url}");
            
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = requestBodyStream;

            await next(context);
            context.Request.Body = originalRequestBody;
        }
    }
}
