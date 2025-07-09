using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicIntensitiesHandler : IChatDynamicNodeHandler
    {
        private readonly IFormulaService _formulaService;

        public DinamicIntensitiesHandler(IFormulaService formulaService)
        {
            _formulaService = formulaService;
        }

        public bool CanHandle(string nodeId) => nodeId == "aprendizaje-intensidades-dinamico";
        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var intensities = await _formulaService.GetIntensitiesAsync();

            var intensitiesDescription = string.Join(" ", intensities.Select(f => $"- {f.Name}: {f.Description} - {f.Category}"));

            var msg = "La <strong>intensidad</strong> de un perfume se refiere a qué tan fuerte y duradero es su aroma. Esto depende de la concentración de esencias aromáticas: a <strong>mayor concentración</strong>, <strong>más intensidad</strong> y <strong>duración</strong>. \nAlgunos ejemplos son: " +
           string.Join("; ", intensities.Select(f => $"{f.Name}: {f.Description} ({f.Category})")) + ".";

            return new ChatNode
            {
                Id = "aprendizaje-intensidades-dinamico",
                Message = msg,
                Type = "decision",
                Options = new List<ChatOption>
                    {
                        new ChatOption { Label = "Volver atrás", NextNodeId = "aprendizaje" },
                        new ChatOption { Label = "Volver al menú principal", NextNodeId = "inicio" }
                    }
            };
        }
    }
}
