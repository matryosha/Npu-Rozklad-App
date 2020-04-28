using System.Collections.Generic;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Telegram.Display.Timetable.SelectingFacultyMenu
{
    public class TimetableFacultyListKeyboardOptions
    {
        public ICollection<Faculty> Faculties { get; set; }
    }
}