using alquimia.Services.Interfaces;
using alquimia.Services.Models;

namespace alquimia.Services.Handler
{
    public class DinamicStateProviderHelp : IChatDynamicNodeHandler
    {
        public bool CanHandle(string nodeId) => nodeId == "proveedor-ayuda-estado-dinamico";

        public async Task<ChatNode?> HandleAsync(string nodeId)
        {
            var msg = "Para conocer el estado de tu cuenta, por favor <strong>ingresá</strong> tu correo electrónico:";

            return new ChatNode
            {
                Id = "proveedor-ayuda-estado-dinamico",
                Message = msg,
                Type = "input",
                Options = new List<ChatOption>
                {
                    new ChatOption { Label = "Volver atrás", NextNodeId = "proveedor-ayuda" },
                    new ChatOption { Label = "Volver al menú", NextNodeId = "inicio" }
                },
                InputType = "email",
                NextNodeId = "proveedor-ayuda-estado-respuesta-dinamico",
            };
        }
    }
}


