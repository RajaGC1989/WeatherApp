using System.Net;

namespace WeatherApp.Application.DTO
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; } 
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

}
