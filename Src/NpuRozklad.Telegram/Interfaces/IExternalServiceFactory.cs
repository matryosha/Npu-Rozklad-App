namespace NpuRozklad.Telegram.Interfaces
{
    public interface IExternalServiceFactory
    {
        T GetService<T>();
    }
}