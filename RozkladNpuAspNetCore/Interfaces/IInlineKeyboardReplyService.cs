using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Interfaces
{
    public interface IInlineKeyboardReplyService
    {
        Task ShowScheduleMenu(Message message);
    }
}
