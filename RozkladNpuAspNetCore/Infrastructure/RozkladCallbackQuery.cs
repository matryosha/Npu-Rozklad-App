namespace RozkladNpuAspNetCore.Infrastructure
{
    //todo better naming
    public enum CallbackQueryType
    {
        AddGroup,
        ShowDetailGroupMenu,
        ShowScheduleMenu
    }

    public enum ShowGroupSelectedWeek
    {
        ThisWeek,
        NextWeek
    }

    public class RozkladCallbackQuery
    {
        public CallbackQueryType UpdateType;
    }
}
