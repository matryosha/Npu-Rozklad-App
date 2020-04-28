using System.Threading.Tasks;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    internal interface ISpecificCallbackQueryHandler
    {
        Task Handle(CallbackQueryData callbackQueryData);
    }
}