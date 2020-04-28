using System.Collections.Generic;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Telegram.Display.Timetable.SelectingFacultyGroupToAddMenu
{
    public class TimetableFacultyGroupsKeyboardOptions
    {
        public ICollection<Group> FacultyGroups { get; set; }
    }
}