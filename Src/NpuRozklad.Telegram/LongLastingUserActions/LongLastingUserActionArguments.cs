using System;
using System.Collections.Generic;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class LongLastingUserActionArguments
    {
        public LongLastingUserActionType UserActionType { get; set; }
        public TelegramRozkladUser TelegramRozkladUser { get; set; }
        public Dictionary<Type, object> Parameters { get; set; } = new Dictionary<Type, object>();
    }
}