using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicHeartNotesHandler : IChatDynamicNodeHandler
    {
        private readonly INoteService _noteService;

        public DinamicHeartNotesHandler(INoteService noteService)
        {
            _noteService = noteService;
        }

        public bool CanHandle(string nodeId) => nodeId == "aprendizaje-notas-corazon-dinamico";

        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var heart = await _noteService.GetNoteNamesBySectorAsync("Corazón");

            var msg = $"Las <strong>notas de corazón</strong> son las que emergen después de que las notas de salida se disipan y forman el <strong>cuerpo principal</strong> del perfume. \nAlgunas de ellas son: {string.Join(", ", heart.Take(3))}";

            return new ChatNode
            {
                Id = "aprendizaje-notas-corazon-dinamico",
                Message = msg,
                Type = "decision",
                Options = new List<ChatOption>
                    {
                    new ChatOption { Label = "Volver atrás", NextNodeId = "aprendizaje-notas-dinamico" },
                        new ChatOption { Label = "Volver al menú principal", NextNodeId = "inicio" }
                    }
            };
        }
    }
}
