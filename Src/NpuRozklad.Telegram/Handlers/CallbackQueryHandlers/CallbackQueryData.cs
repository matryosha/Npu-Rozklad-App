namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers
{
    public class CallbackQueryData
    {
        public string CallbackQueryId { get; set; }
        public CallbackQueryActionType CallbackQueryActionType { get; set; }
        public string[] Values { get; set; }
    }
}