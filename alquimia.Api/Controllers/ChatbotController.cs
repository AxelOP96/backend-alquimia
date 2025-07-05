using alquimia.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace alquimia.Api.Controllers
{
    [ApiController]
    [Route("chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpGet("node/{id}")]
        public async Task<IActionResult> GetNode(string id)
        {
            try
            {
                return Ok(await _chatbotService.GetNodeByIdAsync(id));
            }
            catch (KeyNotFoundException)
            {
                return Ok(await _chatbotService.GetDynamicNodeByIdAsync(id));
            }
        }

        [HttpGet("start")]
        public async Task<IActionResult> Start()
        {
            var node = await _chatbotService.GetNodeByIdAsync("inicio");
            return Ok(node);
        }
    }
}
