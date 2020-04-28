namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers
{
    public class CallbackQueryData
    {
        public CallbackQueryActionType CallbackQueryActionType { get; set; }
        public string[] Values { get; set; }
    }
}