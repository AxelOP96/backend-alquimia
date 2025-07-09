using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IChatDynamicNodeHandler
    {
        bool CanHandle(string nodeId);
        Task<ChatNode?> HandleAsync(string nodeId);
    }
}
