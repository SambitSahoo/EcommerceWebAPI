namespace Catalog.Application.Responses
{
    public class DeleteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public DeleteResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}