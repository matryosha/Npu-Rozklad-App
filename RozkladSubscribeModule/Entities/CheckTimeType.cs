namespace RozkladSubscribeModule.Entities
{
    /// <summary>
    /// Using for selecting timespan to check difference of schedule update
    /// </summary>
    public enum CheckTimeType
    {
        LastDaysOfCurrentWeek,
        LastDaysOfCurrentWeekAndNextWeek,
        OnlyNextDay,
        OnlyTwoNextDays
    }
}