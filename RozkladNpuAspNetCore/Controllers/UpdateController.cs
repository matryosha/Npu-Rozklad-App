using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RozkladNpuAspNetCore.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RozkladNpuAspNetCore.Controllers
{
    [Route("update")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly BotService _botService;
        private readonly MessageHandleService _messageHandleServices;
        private readonly ILogger _logger;
        public UpdateController(BotService service, 
            MessageHandleService messageHandleService,
            ILogger<UpdateController> logger)
        {
            _botService = service;
            _messageHandleServices = messageHandleService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return Ok();
            }

            var message = update.Message;

            _logger.LogInformation("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.Text)
            {
                try
                {
                    await _messageHandleServices.HandleTextMessage(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(4000, e, "Something wrong with message handle service");
                    await _botService.SendErrorMessage(message.Chat.Id);
                    return Ok();
                }
            }
            else if (message.Type == MessageType.Sticker)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Sticker.FileId);
            }
            return Ok();
        }
    }
}