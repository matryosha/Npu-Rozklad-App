namespace NpuRozklad.Telegram
{
    public static class TelegramLogEvents
    {
        public const int UserInitializationError = 5000;
        public const int LongLastingUserActionError = 5010;
        public const int TimetableSelectingFacultyGroupToAddError = 5011;
        public const int TimetableSelectingFacultyError = 5012;
        public const int CallbackQueryHandlerError = 5020;
        public const int TelegramMessageHandlerError = 5030;
        public const int TelegramUserThrottleError = 5040;
    }
}