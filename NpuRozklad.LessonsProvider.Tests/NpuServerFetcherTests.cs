using System.Threading.Tasks;
using NpuRozklad.LessonsProvider.Fetcher;
using NUnit.Framework;

namespace NpuRozklad.LessonsProvider.Tests
{
    public class NpuServerFetcherTests
    {
        private NpuServerFetcher _fetcher;

        [SetUp]
        public void Setup()
        {
            var options = new NpuServerFetcherOptions
            {
                BaseAddress = "https://ei.npu.edu.ua/",
                CallEndPoint = "Server.php",
                RequestOptions = NpuServerFetcherOptions.DefaultNpuRequestFetcherOptions()
            };
            _fetcher = new NpuServerFetcher(options);
        }

        [Test]
        public async Task FetchCalendarTest()
        {
            var result = await _fetcher.FetchCalendar();

            Assert.IsNotEmpty(result);
        }


        [Test]
        public async Task FetchClassroomTest()
        {
            var result = await _fetcher.FetchClassroom("fi");

            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task FetchFacultiesTest()
        {
            var result = await _fetcher.FetchFaculties();

            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task FetchGroupsTest()
        {
            var result = await _fetcher.FetchGroups("fi");

            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task FetchLecturersTest()
        {
            var result = await _fetcher.FetchLecturers("fi");

            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task FetchSettingsTest()
        {
            var result = await _fetcher.FetchSettings();

            Assert.IsNotEmpty(result);
        }
    }
}