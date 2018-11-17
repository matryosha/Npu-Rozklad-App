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
        private MessageService _messageServices;
        public UpdateController(BotService service, MessageService messageService)
        {
            _botService = service;
            _messageServices = messageService;
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
                // Echo each Message
                //_botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
                await _messageServices.TakeMessage(message);

            }
            else if (message.Type == MessageType.Photo)
            {
                //// Download Photo
                //var fileId = message.Photo.LastOrDefault()?.FileId;
                //var file =  _botService.Client.GetFileAsync(fileId);

                //var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                //using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                //{
                //     _botService.Client.DownloadFileAsync(file.FilePath, saveImageStream);
                //}

                // _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }else if (message.Type == MessageType.Sticker)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Sticker.FileId);
            }
            return Ok();
        }
    }
}