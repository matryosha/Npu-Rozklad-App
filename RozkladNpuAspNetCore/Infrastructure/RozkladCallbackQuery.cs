using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RozkladNpuAspNetCore.Infrastructure
{
    public enum CallbackQueryType
    {
        AddGroup,
        ShowGroupMenu
    }

    public enum ShowGroupSelectedWeek
    {
        ThisWeek,
        NextWeek
    }

    public class RozkladCallbackQuery
    {
        public CallbackQueryType UpdateType;
    }
}
