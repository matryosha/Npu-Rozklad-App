using System;
using System.Linq;
using System.Threading.Tasks;
using RozkladNpuBot.Application.Enums;
using RozkladNpuBot.Application.Helpers;
using RozkladNpuBot.Application.Interfaces;
using RozkladNpuBot.Domain.Entities;
using RozkladNpuBot.Infrastructure;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Services
{
    public class CallbackQueryHandlerService : ICallbackQueryHandlerService
    {
        private readonly IKeyboardReplyService _keyboardReplyService;
        private readonly IInlineKeyboardReplyService _inlineKeyboardReplyService;
        private readonly IUserService _userService;
        private readonly IBotService _botService;

        public CallbackQueryHandlerService(
            IKeyboardReplyService keyboardReplyService,
            IInlineKeyboardReplyService inlineKeyboardReplyService,
            IUserService userService,
            IBotService botService)
        {
            _keyboardReplyService = keyboardReplyService;
            _inlineKeyboardReplyService = inlineKeyboardReplyService;
            _userService = userService;
            _botService = botService;
        }

        public async Task Handle(CallbackQuery callbackQuery)
        {
            if (string.IsNullOrEmpty(callbackQuery.Data)) return;
            var callbackQueryKeyValuePair =
                CallbackQueryDataConverters.ConvertDataFromString(callbackQuery.Data);
            await _botService.Client.AnswerCallbackQueryAsync(callbackQuery.Id);
            switch (callbackQueryKeyValuePair.Key)
            {
                case CallbackQueryType.AddGroup:
                {
                    var user =
                        await _userService.GetUser(callbackQuery.From.Id)
                            .ConfigureAwait(false);
                    user.LastAction = RozkladUser.LastActionType.WaitForFaculty;
                    await _userService.UpdateUser(user);
                    await _keyboardReplyService.ShowFacultyList(callbackQuery.Message);
                    break;
                }
                case CallbackQueryType.ShowDetailGroupMenu:
                {
                    await _inlineKeyboardReplyService.ShowGroupMenu(
                        callbackQuery.Message,
                        CallbackQueryDataConverters.ToGroup(callbackQueryKeyValuePair.Value),
                        (DayOfWeek) int.Parse(callbackQueryKeyValuePair.Value[3]),
                        (ShowGroupSelectedWeek) int.Parse(callbackQueryKeyValuePair.Value[4]),
                        int.Parse(callbackQueryKeyValuePair.Value[5]));
                    break;
                }
                case CallbackQueryType.ShowScheduleMenu:
                {
                    await _inlineKeyboardReplyService
                        .ShowScheduleMenu(callbackQuery.Message, callbackQuery.From.Id);
                    break;
                }
                case CallbackQueryType.DeleteGroup:
                {
                    var user = await _userService.GetUser(
                        int.Parse(callbackQueryKeyValuePair.Value[0]));

                    user.Groups.Remove(
                        user.Groups.FirstOrDefault(
                            g => g.ExternalId == int.Parse(
                                     callbackQueryKeyValuePair.Value[1])));

                    await _userService.UpdateUser(user);
                    await _inlineKeyboardReplyService
                        .ShowScheduleMenu(callbackQuery.Message, callbackQuery.From.Id);
                    break;
                }
            }
        }
    }
}
