using System.IO;

namespace NpuRozklad.LessonsProvider.Tests.Infrastructure
{
    public static class StubReader
    {
        public static string ReadCalendar() => ReadFileContent("CalendarRawContent.txt");
        public static string ReadClassrooms()=> ReadFileContent("ClassroomsRawContent.txt");
        public static string ReadFaculties()=> ReadFileContent("FacultiesRawContent.txt");
        public static string ReadGroups()=> ReadFileContent("GroupsRawContent.txt");
        public static string ReadLecturers()=> ReadFileContent("LecturesRawContent.txt");
        public static string ReadSettings()=> ReadFileContent("SettingsRawContent.txt");

        private static string ReadFileContent(string fileName)
        {
            var dirName = Path.Combine("Infrastructure", "StubContent");
            return File.ReadAllText(Path.Combine(dirName, fileName));
        }
    }
}