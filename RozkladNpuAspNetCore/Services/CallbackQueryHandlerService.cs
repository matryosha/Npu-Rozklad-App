using System;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Entities;
using RozkladNpuAspNetCore.Infrastructure;
using RozkladNpuAspNetCore.Interfaces;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Services
{
    public class CallbackQueryHandlerService : ICallbackQueryHandlerService
    {
        private readonly IKeyboardReplyService _keyboardReplyService;
        private readonly IInlineKeyboardReplyService _inlineKeyboardReplyService;
        private readonly IUserService _userService;
        private readonly BotService _botService;

        public CallbackQueryHandlerService(
            IKeyboardReplyService keyboardReplyService,
            IInlineKeyboardReplyService inlineKeyboardReplyService,
            IUserService userService,
            BotService botService)
        {
            _keyboardReplyService = keyboardReplyService;
            _inlineKeyboardReplyService = inlineKeyboardReplyService;
            _userService = userService;
            _botService = botService;
        }

        public async Task Handle(CallbackQuery callbackQuery)
        {
            if(string.IsNullOrEmpty(callbackQuery.Data)) return;    
            var callbackQueryData = CallbackQueryDataConverters.ConvertDataFromString(callbackQuery.Data);
            await _botService.Client.AnswerCallbackQueryAsync(callbackQuery.Id);
            switch (callbackQueryData.Key)
            {
                case CallbackQueryType.AddGroup:
                {
                    var user = await _userService.GetUser(callbackQuery.From.Id).ConfigureAwait(false);
                    user.LastAction = RozkladUser.LastActionType.WaitForFaculty;
                    await _userService.UpdateUser(user);
                    await _keyboardReplyService.ShowFacultyList(callbackQuery.Message);
                    break;
                }
                case CallbackQueryType.ShowDetailGroupMenu:
                {
                    await _inlineKeyboardReplyService.ShowGroupMenu(
                        callbackQuery.Message,
                        CallbackQueryDataConverters.ToGroup(callbackQueryData.Value),
                        (DayOfWeek)int.Parse(callbackQueryData.Value[3]),
                        (ShowGroupSelectedWeek)int.Parse(callbackQueryData.Value[4]));
                    break;
                }
                case CallbackQueryType.ShowScheduleMenu:
                {
                    await _inlineKeyboardReplyService.ShowScheduleMenu(callbackQuery.Message, callbackQuery.From.Id);
                    break;
                }

            }

        }
    }
}
