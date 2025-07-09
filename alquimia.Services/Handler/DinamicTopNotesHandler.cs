using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicTopNotesHandler : IChatDynamicNodeHandler
    {
        private readonly INoteService _noteService;

        public DinamicTopNotesHandler(INoteService noteService)
        {
            _noteService = noteService;
        }

        public bool CanHandle(string nodeId) => nodeId == "aprendizaje-notas-salida-dinamico";

        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var top = await _noteService.GetNoteNamesBySectorAsync("Salida");

            var msg = $" Las <strong>notas de salida</strong> son las primeras que se perciben al aplicar un perfume. " +
                $"Son <strong>ligeras</strong> y <strong>volátiles</strong>, como {string.Join(", ", top.Take(3))}";

            return new ChatNode
            {
                Id = "aprendizaje-notas-salida-dinamico",
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
