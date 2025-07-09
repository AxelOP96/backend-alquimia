using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicNotesHandler : IChatDynamicNodeHandler
    {
        public bool CanHandle(string nodeId) => nodeId == "aprendizaje-notas-dinamico";

        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var msg = "Una <strong>nota</strong> es uno de los aromas individuales que se combinan para crear un perfume. Son los distintos <strong>olores</strong> que vas percibiendo desde que lo aplicás hasta que pasa el tiempo. \nLas notas se clasifican en <strong>notas de fondo, notas de corazón</strong> y <strong>notas de salida</strong>";

            return new ChatNode
            {
                Id = "aprendizaje-notas-dinamico",
                Message = msg,
                Type = "decision",
                Options = new List<ChatOption>
                    {
                        new ChatOption { Label = "Quiero conocer sobre las notas de fondo", NextNodeId = "aprendizaje-notas-fondo-dinamico" },
                        new ChatOption { Label = "Quiero conocer sobre las notas de corazón", NextNodeId = "aprendizaje-notas-corazon-dinamico" },
                        new ChatOption { Label = "Quiero conocer sobre las notas de salida", NextNodeId = "aprendizaje-notas-salida-dinamico" },
                        new ChatOption { Label = "Volver atrás", NextNodeId = "aprendizaje" },
                        new ChatOption { Label = "Volver al menú principal", NextNodeId = "inicio" }
                    }
            };
        }
    }
}
