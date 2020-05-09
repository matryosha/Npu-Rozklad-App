namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ICurrentTelegramUserProvider
    {
        TelegramRozkladUser GetCurrentTelegramRozkladUser();
    }
}