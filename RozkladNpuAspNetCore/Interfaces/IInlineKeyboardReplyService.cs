using System;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Entities;
using RozkladNpuAspNetCore.Infrastructure;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Interfaces
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
