using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpuTimetableParser;

namespace NpuTimeTableParserTest
{
    [TestClass]
    public class NpuParserTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            NpuParser parser = new NpuParser();
            //var calendarRaw = parser.FillCalendarRawList();
            //var groups = parser.FillGroupList();
            //var lecturers = parser.FillLecturersList();
            //var classrooms = parser.FillClassroomsList();
            var lessons = parser.CreateLessonsList();

        }
    }
}
