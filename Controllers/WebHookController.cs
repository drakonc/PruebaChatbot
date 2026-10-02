using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaChatbot.Models;

namespace PruebaChatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        public BotService _bot { get; }
        public ILogger<WebHookController> _logger { get; }
        
        public WebHookController(BotService bot, ILogger<WebHookController> logger)
        {
            _bot = bot;
            _logger = logger;
        }

        [HttpPost("whatsapp")]
        public async Task<ActionResult<bool>> WhatsApp([FromBody] Payload payload) {

            var message = payload.entry.FirstOrDefault()?.changes?.FirstOrDefault()?.value?.messages?.FirstOrDefault();
            
            if(message?.type != "text" || message.from is null || message.text?.body is null)
                return await Task.FromResult(false);

            var from = message.from;
            var id = message.id;
            var text = message.text.body;

            _logger.LogInformation($"From:{from} Id:{id} Text:{text}");

            await _bot.ProcesamientoAsync(from, text);
            return await Task.FromResult(true);
        }

    }
}
