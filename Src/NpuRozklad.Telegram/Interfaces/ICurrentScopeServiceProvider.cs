namespace NpuRozklad.Telegram.Interfaces
{
    public interface ICurrentScopeServiceProvider
    {
        T GetService<T>();
    }
}