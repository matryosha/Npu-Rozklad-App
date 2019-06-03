using System;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuBot.Application.Enums;
using RozkladNpuBot.Domain.Entities;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Interfaces
{
    public interface IInlineKeyboardReplyService
    {
        Task<Message> ShowScheduleMenu(Message message, int telegramId);
        Task<Message> ShowScheduleMenu(
            Message message, 
            RozkladUser user,
            bool spawnNewMenu = false);
        Task<Message> ShowGroupMenu(
            Message callbackQueryMessage, 
            Group group,
            DayOfWeek dayOfWeek,
            ShowGroupSelectedWeek week,
            int userTelegramId,
            bool isSingleGroup = false,
            bool spawnNewMenu = false);
    }
}
