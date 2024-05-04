namespace MenuApp.BLL.Services
{
    namespace MenuApp.BLL.Services
    {
        public class ServiceResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object? Data { get; set; }

            public ServiceResult(bool success, string message, object? data = null)
            {
                Success = success;
                Message = message;
                Data = data;
            }
        }
    }
}
