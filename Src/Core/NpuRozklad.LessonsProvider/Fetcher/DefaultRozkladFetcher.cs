using System.Collections.Generic;

namespace NpuRozklad.Parser.Fetcher
{
    public class DefaultRozkladFetcher : IRozkladFetcher
    {
        public Task<List<Lesson>> FetchLessons()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Group>> FetchGroups()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Classroom>> FetchClassrooms()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Lecturer>> FetchLecturers()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Faculty>> FetchFaculties()
        {
            throw new System.NotImplementedException();
        }
    }
}