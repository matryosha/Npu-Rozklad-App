using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NpuRozklad.Telegram.Handlers;
using Telegram.Bot.Types;

namespace NpuRozklad.Web
{
    [ApiController]
    [Route("/telegram")]
    public class TelegramEndpoint : ControllerBase
    {
        private readonly ILogger<TelegramEndpoint> _logger;
        private readonly ITelegramUpdateHandler _telegramUpdateHandler;

        public TelegramEndpoint(
            ILogger<TelegramEndpoint> logger,
            ITelegramUpdateHandler telegramUpdateHandler)
        {
            _logger = logger;
            _telegramUpdateHandler = telegramUpdateHandler;
        }

        [HttpPost]
        public async Task<IActionResult> TelegramUpdate(Update update)
        {
            await _telegramUpdateHandler.Handle(update);

            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult("Ok");
        }
    }
}