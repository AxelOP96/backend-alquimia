using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicBaseNotesHandler : IChatDynamicNodeHandler
    {
        private readonly INoteService _noteService;

        public DinamicBaseNotesHandler(INoteService noteService)
        {
            _noteService = noteService;
        }

        public bool CanHandle(string nodeId) => nodeId == "aprendizaje-notas-fondo-dinamico";

        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var _base = await _noteService.GetNoteNamesBySectorAsync("Fondo");

            var msg = $"Las <strong>notas de fondo</strong> son las últimas notas que se perciben en un perfume y son las más <strong>duraderas</strong>, permaneciendo en la piel durante varias horas. \nAlgunas de ellas son: {string.Join(", ", _base.Take(3))}";

            return new ChatNode
            {
                Id = "aprendizaje-notas-fondo-dinamico",
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
