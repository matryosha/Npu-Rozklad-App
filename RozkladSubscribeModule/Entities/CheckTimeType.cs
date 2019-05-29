namespace RozkladSubscribeModule.Entities
{
    /// <summary>
    /// Using for selecting timespan to check difference of schedule update
    /// </summary>
    internal enum CheckTimeType
    {
        LastDaysOfCurrentWeek,
        LastDaysOfCurrentWeekAndNextWeek,
        OnlyNextDay,
        OnlyTwoNextDays
    }
}