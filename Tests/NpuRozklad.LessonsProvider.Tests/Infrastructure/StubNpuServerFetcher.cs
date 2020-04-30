using Moq;
using NpuRozklad.LessonsProvider.Fetcher;
using static NpuRozklad.LessonsProvider.Tests.Infrastructure.StubReader;

namespace NpuRozklad.LessonsProvider.Tests.Infrastructure
{
    public static class StubNpuServerFetcher
    {
        public static INpuServerFetcher Fetcher;

        static StubNpuServerFetcher()
        {
            var mock = new Mock<INpuServerFetcher>();
            mock.Setup(f => f.FetchCalendar())
                .ReturnsAsync(ReadCalendar);
            var faculty = "fi";

            mock.Setup(f => f.FetchClassroom(It.Is<string>(s => s == faculty)))
                .ReturnsAsync(ReadClassrooms);
            mock.Setup(f => f.FetchFaculties())
                .ReturnsAsync(ReadFaculties);
            mock.Setup(f => f.FetchGroups(It.Is<string>(s => s == faculty)))
                .ReturnsAsync(ReadGroups);
            mock.Setup(f => f.FetchLecturers(It.Is<string>(s => s == faculty)))
                .ReturnsAsync(ReadLecturers);
            mock.Setup(f => f.FetchSettings())
                .ReturnsAsync(ReadSettings);
            Fetcher = mock.Object;
        }
    }
}