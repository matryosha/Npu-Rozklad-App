namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ICurrentScopeMessageIdProvider
    {
        int? MessageId { get; set; }
    }
}