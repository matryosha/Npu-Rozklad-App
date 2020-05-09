using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Services
{
    public class CurrentScopeMessageIdProvider : ICurrentScopeMessageIdProvider
    {
        public int? MessageId { get; set; }
    }
}