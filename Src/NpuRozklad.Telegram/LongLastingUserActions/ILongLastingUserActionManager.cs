using System.Threading.Tasks;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public interface ILongLastingUserActionManager
    {
        Task UpsertUserAction(LongLastingUserActionArguments arguments);
        Task ClearUserAction(TelegramRozkladUser rozkladUser);
        Task<LongLastingUserActionArguments> GetUserLongLastingAction(TelegramRozkladUser telegramRozkladUser);
    }
}