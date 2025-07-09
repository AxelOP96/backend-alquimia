namespace alquimia.Services.Models
{
    public class ChatNode
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<ChatOption> Options { get; set; } = new();
        public string Type { get; set; } = "decision"; // "decision", "input", "generated", etc.
        public string? InputType { get; set; } // Ej: "text", "email", "number"
        public string? NextNodeId { get; set; } // Para los inputs: a dónde ir luego de ingresar el valor
    }
}

