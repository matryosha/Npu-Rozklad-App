using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Entities;
using RozkladNpuAspNetCore.Infrastructure;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Interfaces
{
    public interface IInlineKeyboardReplyService
    {
        Task ShowScheduleMenu(Message message, int telegramId);
        Task ShowScheduleMenu(
            Message message, 
            RozkladUser user,
            bool spawnNewMenu = false);
        Task ShowGroupMenu(
            Message callbackQueryMessage, 
            Group group,
            DayOfWeek dayOfWeek,
            ShowGroupSelectedWeek week,
            bool isSingleGroup = false,
            bool spawnNewMenu = false);
    }
}
