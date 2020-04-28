using System.Threading.Tasks;

namespace NpuRozklad.LessonsProvider.Fetcher
{
    public interface INpuServerFetcher
    {
        Task<string> FetchCalendar();
        Task<string> FetchSettings();
        Task<string> FetchFaculties();
        Task<string> FetchLecturers(string facultyTypeid);
        Task<string> FetchClassroom(string facultyTypeid);
        Task<string> FetchGroups(string facultyTypeid);
    }
}