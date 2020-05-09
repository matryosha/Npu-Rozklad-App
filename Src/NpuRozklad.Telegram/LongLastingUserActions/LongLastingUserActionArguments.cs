using System;
using System.Collections.Generic;
using System.Linq;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class LongLastingUserActionArguments
    {
        public LongLastingUserActionType UserActionType { get; set; }
        public TelegramRozkladUser TelegramRozkladUser { get; set; }
        public Dictionary<Type, object> Parameters { get; set; } = new Dictionary<Type, object>();

        public override string ToString() =>
            $"{nameof(UserActionType)}: {UserActionType}. {nameof(TelegramRozkladUser)}: {TelegramRozkladUser}. " +
            $"{nameof(Parameters)}: {string.Join(";", Parameters.Select(x => x.Key + "=" + x.Value).ToString())}";
    }
}