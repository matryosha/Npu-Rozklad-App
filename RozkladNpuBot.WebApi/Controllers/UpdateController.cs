using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RozkladNpuBot.Application.Interfaces;
using RozkladNpuBot.Infrastructure;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RozkladNpuBot.WebApi.Controllers
{
    [Route("update")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IBotService _botService;
        private readonly IMessageHandleService _messageHandleServices;
        private readonly ICallbackQueryHandlerService _callbackQueryHandlerService;
        private readonly ILogger _logger;
        public UpdateController(IBotService service,
            IMessageHandleService messageHandleService,
            ICallbackQueryHandlerService callbackQueryHandlerService,
            ILogger<UpdateController> logger)
        {
            _botService = service;
            _messageHandleServices = messageHandleService;
            _callbackQueryHandlerService = callbackQueryHandlerService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message;

                    _logger.LogInformation("Received Message from {0}", message.Chat.Id);

                    if (message.Type == MessageType.Text)
                    {
                        try
                        {
                            await _messageHandleServices.HandleMessage(message);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(4000, e, "Something wrong with message handle service");
                            await _botService.SendErrorMessage(message.Chat.Id);
                        }
                    }
                    else if (message.Type == MessageType.Sticker)
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Sticker.FileId);
                    }

                    break;
                }
                case UpdateType.CallbackQuery:
                {
                    var callbackQuery = update.CallbackQuery;

                    _logger.LogInformation("Received CallbackQuery from {0}", callbackQuery.Message.Chat.Id);
                    try
                    {
                        await _callbackQueryHandlerService.Handle(callbackQuery);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(5000, e, "Something wrong with message handle service");
                        await _botService.SendErrorMessage(callbackQuery.Message.Chat.Id);
                    }
                    break;
                }
            }

            return Ok();
        }
    }
}