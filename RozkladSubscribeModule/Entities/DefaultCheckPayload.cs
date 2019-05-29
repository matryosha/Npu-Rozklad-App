using System;
using System.Collections.Generic;
using System.Linq;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Entities
{
    internal class DefaultCheckPayload : ICheckPayload
    {
        private readonly HashSet<DateTime> _dateWithNewSchedule;
        public DefaultCheckPayload()
        {
            _dateWithNewSchedule = new HashSet<DateTime>();
        }
        public bool IsDiff() => _dateWithNewSchedule.Any();

        public void AddDateWithNewSchedule(DateTime dateTime)
        {
            _dateWithNewSchedule.Add(dateTime);
        }
    }
}