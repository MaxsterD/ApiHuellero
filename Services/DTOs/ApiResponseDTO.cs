namespace ConsolaBlazor.Services.DTOs
{
    public class ApiResponseDTO
    {
        public bool Success { get; set; } = false;
        public string? Message { get; set; } = null;
        public string? Mensaje { get; set; } = null;
        public object? Data { get; set; }
    }
}
