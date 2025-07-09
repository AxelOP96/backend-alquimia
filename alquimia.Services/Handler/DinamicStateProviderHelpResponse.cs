using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicStateProviderHelpResponse : IChatDynamicNodeHandler
    {
        private readonly IAdminService _adminService;

        public DinamicStateProviderHelpResponse(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public bool CanHandle(string nodeId) => nodeId.StartsWith("proveedor-ayuda-estado-respuesta-dinamico");

        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var email = nodeId.Split("::").Length > 1 ? nodeId.Split("::")[1] : null;

            if (string.IsNullOrEmpty(email))
            {
                return new ChatNode
                {
                    Id = "proveedor-ayuda-estado-respuesta-dinamico",
                    Message = "No se recibió un correo válido. Por favor, intentá de nuevo.",
                    Type = "decision",
                    Options = new List<ChatOption>
                {
                    new ChatOption { Label = "Reintentar", NextNodeId = "proveedor-ayuda-estado-dinamico" },
                    new ChatOption { Label = "Volver al menú", NextNodeId = "inicio" }
                }
                };
            }

            var provider = await _adminService.GetPendingOrApprovedProviderByEmailAsync(email);
            var textState = provider.EsAprobado ? "<strong>aprobado</strong>" : "<strong>pendiente de aprobación</strong>";

            return new ChatNode
            {
                Id = "proveedor-ayuda-estado-respuesta-dinamico",
                Message = $"El estado de tu cuenta es: {textState}",
                Type = "decision",
                Options = new List<ChatOption>
            {
                new ChatOption { Label = "Volver atrás", NextNodeId = "proveedor-ayuda" },
                new ChatOption { Label = "Volver al menú", NextNodeId = "inicio" }
            }
            };
        }
    }
}
