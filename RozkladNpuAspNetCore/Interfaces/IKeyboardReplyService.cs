using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuTimetableParser;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Interfaces
{
    public interface IKeyboardReplyService
    {
        Task ShowMainMenu(Message message);
        Task ShowScheduleMenu(Message message);
        Task ShowFacultyList(Message message);
        //Returns true if groups exist for given faculty and show them
        Task<bool> ShowGroupList(Message message, Faculty faculty);
    }
}
