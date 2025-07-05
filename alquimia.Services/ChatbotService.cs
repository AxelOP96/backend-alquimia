using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace alquimia.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly Dictionary<string, ChatNode> _staticNodes;
        private readonly IEnumerable<IChatDynamicNodeHandler> _handlers;
        private readonly INoteService _noteService;
        private readonly IConfiguration _config;

        public ChatbotService(IWebHostEnvironment env, IEnumerable<IChatDynamicNodeHandler> handlers, IConfiguration config)
        {
            _handlers = handlers;
            _config = config;

            var path = Path.Combine(env.ContentRootPath, "Data", "chatFlow.json");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var frontendUrl = _config["AppSettings:FrontendBaseUrl"];
                json = json.Replace("{{FrontendBaseUrl}}", frontendUrl);
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };
                    var nodeList = JsonSerializer.Deserialize<List<ChatNode>>(json, options);
                    foreach (var node in nodeList)
                    {
                        if (node.Options == null)
                            node.Options = new List<ChatOption>();
                    }
                    ValidateChatNodes(nodeList);

                    var invalidIds = nodeList!
                        .Where(n => string.IsNullOrWhiteSpace(n.Id))
                        .Select(n => n.Id ?? "<null>")
                        .ToList();

                    if (invalidIds.Any())
                    {
                        throw new Exception("IDs inválidos (vacíos o nulos) detectados.");
                    }

                    var duplicatedIds = nodeList!
                        .GroupBy(n => n.Id)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicatedIds.Any())
                    {
                        throw new Exception("IDs duplicados detectados: " + string.Join(", ", duplicatedIds));
                    }

                    _staticNodes = nodeList.ToDictionary(n => n.Id, n => n);
                }
                catch (Exception ex)
                {
                    _staticNodes = new();
                }
            }
            else
            {
                _staticNodes = new();
            }
        }

        public Task<ChatNode?> GetNodeByIdAsync(string id)
        {
            if (!_staticNodes.TryGetValue(id, out var node))
                throw new KeyNotFoundException($"Nodo '{id}' no encontrado");

            return Task.FromResult(node);
        }

        public async Task<ChatNode> GetDynamicNodeByIdAsync(string id)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(id));
            if (handler != null)
                return await handler.HandleAsync(id);

            throw new KeyNotFoundException($"Nodo dinámico '{id}' no encontrado");
        }

        private void ValidateChatNodes(List<ChatNode> nodeList)
        {
            var duplicatedIds = nodeList
                .GroupBy(n => n.Id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatedIds.Any())
            {
                throw new Exception("IDs duplicados en chatFlow.json");
            }

            var emptyIds = nodeList
                .Where(n => string.IsNullOrWhiteSpace(n.Id))
                .Select((n, i) => $"Nodo #{i + 1}")
                .ToList();

            if (emptyIds.Any())
            {
                throw new Exception("Nodos con ID inválido en chatFlow.json");
            }

            var invalidOptions = nodeList
                .SelectMany(n => n.Options ?? new List<ChatOption>())
                .Where(opt => string.IsNullOrWhiteSpace(opt.Label) || string.IsNullOrWhiteSpace(opt.NextNodeId))
                .ToList();
        }
    }
}
