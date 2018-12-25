using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RozkladNpuAspNetCore.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RozkladNpuAspNetCore.Controllers
{
    [Route("update")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private BotService _botService;
        private MessageHandleService _messageHandleServices;
        public UpdateController(BotService service, MessageHandleService messageHandleService)
        {
            _botService = service;
            _messageHandleServices = messageHandleService;
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return Ok();
            }

            var message = update.Message;

            //_logger.LogInformation("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.Text)
            {
                await _messageHandleServices.HandleTextMessage(message);

            }
            else if (message.Type == MessageType.Sticker)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Sticker.FileId);
            }
            return Ok();
        }
    }
}