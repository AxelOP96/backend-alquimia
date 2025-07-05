using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IChatbotService
    {
        Task<ChatNode?> GetNodeByIdAsync(string id);
        Task<ChatNode?> GetDynamicNodeByIdAsync(string id);
    }

}
