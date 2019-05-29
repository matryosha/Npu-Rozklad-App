using System;
using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    /// <summary>
    /// Returns needed dates specified with <see cref="CheckTimeType"/>
    /// </summary>
    internal interface IDateTimesForScheduleDiffCheckGiver
    {
        List<DateTime> GetDates();
    }
}