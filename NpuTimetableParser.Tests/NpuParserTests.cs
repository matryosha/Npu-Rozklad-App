using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpuTimeTableParserTest.Infrastructure;

namespace NpuTimetableParser.Tests
{
    [TestClass]
    public class NpuParserTests
    {
        private static string _mockServerContentPath = "Infrastructure/MockServerContent";

        [TestMethod]
        public async Task SimpleLessonsTest()
        {
            //arrange
            var parser = GetParser();
            var groups = await parser.GetGroups("fi");
            
            var group = groups.FirstOrDefault(g => g.ExternalId == 75);
            //act
            List<Lesson> testLessonsList =
                await parser.GetLessonsOnDate("fi", group.ExternalId, new DateTime(2019, 3, 18));

            //assert
            Assert.AreEqual(3, testLessonsList.Count);

            var assert1 = testLessonsList.Where(l => l.LessonNumber == 2).ToList();
            Assert.AreEqual(1, assert1.Count);
            var lesson1 = assert1[0];
            Assert.AreEqual(896, lesson1.Lecturer.ExternalId);
            Assert.AreEqual(Fraction.None, lesson1.Fraction);
            Assert.AreEqual(SubGroup.None, lesson1.SubGroup);

            var assert2 = testLessonsList.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(1, assert2.Count);
            var lesson2 = assert2[0];
            Assert.AreEqual(896, lesson2.Lecturer.ExternalId);
            Assert.AreEqual(Fraction.None, lesson2.Fraction);
            Assert.AreEqual(SubGroup.None, lesson2.SubGroup);

            var assert3 = testLessonsList.Where(l => l.LessonNumber == 4).ToList();
            Assert.AreEqual(1, assert3.Count);
            var lesson3 = assert3[0];
            Assert.AreEqual(115, lesson3.Lecturer.ExternalId);
            Assert.AreEqual(Fraction.None, lesson3.Fraction);
            Assert.AreEqual(SubGroup.None, lesson3.SubGroup);
        }

        [TestMethod]
        public async Task SimpleFractionTest()
        {
            var parser = GetParser();
            var groups = await parser.GetGroups("fi");

            var group = groups.FirstOrDefault(g => g.ExternalId == 411);
            List<Lesson> lessons =
                await parser.GetLessonsOnDate("fi", group.ExternalId, new DateTime(2019, 3, 18));

            Assert.AreEqual(4, lessons.Count);
            var fractionLesson = lessons.FirstOrDefault(l => l.LessonNumber == 3);
            Assert.AreEqual(Fraction.Numerator, fractionLesson.Fraction);
        }

        [TestMethod]
        public async Task DoubleFractionTest()
        {
            var parser = GetParser();
            var groups = await parser.GetGroups("fi");
            var group = groups.FirstOrDefault(g => g.ExternalId == 86);

            List<Lesson> lessons =
                await parser.GetLessonsOnDate("fi", group.ExternalId, new DateTime(2019, 3, 19));

            Assert.AreEqual(2, lessons.Count);
            var classes3 = lessons.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(1, classes3.Count);
            var class3 = classes3.FirstOrDefault();
            Assert.AreEqual(Fraction.Numerator, class3.Fraction);
            Assert.AreEqual("Комп'ютерна графіка", class3.Subject.Name);
        }

        [TestMethod]
        public async Task SubGroupTest()
        {
            var parser = GetParser();
            var groups = await parser.GetGroups("fi");

            var group = groups.FirstOrDefault(g => g.ExternalId == 490);

            List<Lesson> lessons =
                await parser.GetLessonsOnDate("fi", group.ExternalId, new DateTime(2019, 3, 20));

            Assert.AreEqual(5, lessons.Count);
            var subGroupLessons = lessons.Where(l => l.LessonNumber == 4).ToList();
            Assert.AreEqual(2, subGroupLessons.Count);
            var firstGroupLesson = subGroupLessons.FirstOrDefault(l => l.SubGroup == SubGroup.First);
            var secondGroupLesson = subGroupLessons.FirstOrDefault(l => l.SubGroup == SubGroup.Second);
            Assert.AreEqual("Економіка та бізнес (за вибором)", firstGroupLesson.Subject.Name);
            Assert.AreEqual("Політологія", secondGroupLesson.Subject.Name);
        }

        [TestMethod]
        public void RealServer_FacultiesNotEmpty()
        {
            var parser = new NpuParser();
            var faculties = parser.GetFaculties();
            Assert.IsTrue(faculties.Any());
        }

        [TestMethod]
        public async Task RealServer_GroupsNotEmpty()
        {
            var parser = new NpuParser();
            var groups = await parser.GetGroups("fi");
            Assert.IsTrue(groups.Any());
        }

        [TestMethod]
        public async Task RealServer_LessonsNotEmpty()
        {
            var parser = new NpuParser();
            var groups = await parser.GetGroups("fi");
            var lessons = await parser.GetLessonsOnDate("fi", groups[4].ExternalId, new DateTime(2019, 11, 5));
            Assert.IsTrue(lessons.Any());
        }

        public static string ReadMockContent(string fileName)
        {
            return File.ReadAllText($"{fileName}");
        }

        private static NpuParser GetParser()
        {
            var mockClient = new MockRestClient
            {
                CalendarRawContent = ReadMockContent($"{_mockServerContentPath}/CalendarRawContent.txt"),
                GroupsRawContent = ReadMockContent($"{_mockServerContentPath}/GroupsRawContent.txt"),
                LecturesRawContent = ReadMockContent($"{_mockServerContentPath}/LecturesRawContent.txt"),
                ClassroomsRawContent = ReadMockContent($"{_mockServerContentPath}/ClassroomsRawContent.txt"),
                FacultiesRawContent = ReadMockContent($"{_mockServerContentPath}/FacultiesRawContent.txt"),
                SettingsRawContent = ReadMockContent($"{_mockServerContentPath}/SettingsRawContent.txt")
            };
            var parser = new NpuParser();
            parser.SetClient(mockClient);
            return parser;
        }
    }
}