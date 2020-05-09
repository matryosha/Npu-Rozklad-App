using System.Threading.Tasks;

namespace NpuRozklad.Telegram.Persistence
{
    public interface ITelegramRozkladUserDao
    {
        Task<TelegramRozkladUser> FindByTelegramId(int telegramId);
        Task Add(TelegramRozkladUser telegramRozkladUser);
        Task Delete(TelegramRozkladUser telegramRozkladUser);
        Task Update(TelegramRozkladUser telegramRozkladUser);
    }
}