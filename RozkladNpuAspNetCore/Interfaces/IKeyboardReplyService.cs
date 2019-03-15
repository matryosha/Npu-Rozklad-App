using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Interfaces
{
    public interface IKeyboardReplyService
    {
        Task ShowMainMenu(Message message);
        Task ShowScheduleMenu(Message message);
    }
}
