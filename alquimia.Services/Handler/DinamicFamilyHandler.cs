using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicFamilyHandler : IChatDynamicNodeHandler
    {
        private readonly IOlfactoryFamilyService _familyService;

        public DinamicFamilyHandler(IOlfactoryFamilyService familyService)
        {
            _familyService = familyService;
        }

        public bool CanHandle(string nodeId) => nodeId == "aprendizaje-familias-dinamico";
        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var families = await _familyService.GetAllFamilies();

            var randomFamilies = families
                .OrderBy(f => Guid.NewGuid())
                .Take(5);

            var familyDescriptions = string.Join(" ", randomFamilies.Select(f => $"- {f.Name}: {f.Description}"));

            var msg = "Las <strong>familias olfativas</strong> son categorías que agrupan las fragancias según sus <strong>características aromáticas</strong> predominantes. Es decir, que categorizan a las notas en grupos. Algunas de ellas son: " + familyDescriptions;

            return new ChatNode
            {
                Id = "aprendizaje-familias-dinamico",
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
