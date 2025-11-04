using System.Net;

namespace pva.SuperV.Blazor.Services
{
    public class ApiException : Exception
    {
        private readonly string? message;
        private readonly string? status;

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(Exception e) : base("API error", e)
        {
        }

        public ApiException(HttpStatusCode statusCode, HttpContent content)
        {
            message = content.ReadAsStringAsync().Result;
            status = statusCode switch
            {
                HttpStatusCode.NotFound => "Not found",
                HttpStatusCode.BadRequest => "Bad request",
                HttpStatusCode.InternalServerError => "Internal server error",
                _ => statusCode.ToString()
            };
        }
        public override string Message
            => status switch
            {
                not null => $"{status}: {message}",
                _ => base.Message
            };
    }
}
