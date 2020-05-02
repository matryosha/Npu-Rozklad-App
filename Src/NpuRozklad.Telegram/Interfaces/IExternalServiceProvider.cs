namespace NpuRozklad.Telegram.Interfaces
{
    public interface IExternalServiceProvider
    {
        T GetService<T>();
    }
}