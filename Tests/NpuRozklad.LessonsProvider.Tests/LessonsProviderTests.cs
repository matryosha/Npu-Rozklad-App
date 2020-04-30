using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Holders;
using NpuRozklad.LessonsProvider.Holders.Interfaces;
using NpuRozklad.LessonsProvider.Tests.Infrastructure;
using NUnit.Framework;

namespace NpuRozklad.LessonsProvider.Tests
{
    public class LessonsProviderTests
    {
        private LessonProvider _lessonProvider;
        private FacultiesProvider facultiesProvider;
        private IGroupsHolder groupsHolder;

        [SetUp]
        public void Setup()
        {
            var stubFetcher = StubNpuServerFetcher.Fetcher;
            facultiesProvider = new FacultiesProvider(stubFetcher);
            groupsHolder = new GroupsHolder(stubFetcher, facultiesProvider);
            LecturersHolder lecturersHolder = new LecturersHolder(stubFetcher);
            ClassroomHolder classroomHolder = new ClassroomHolder(stubFetcher);
            CalendarRawItemHolder calendarRawItemHolder = new CalendarRawItemHolder(stubFetcher);
            SettingsHolder settingsHolder = new SettingsHolder(stubFetcher);
            OddDayWeekChecker oddDayWeekChecker = new OddDayWeekChecker(settingsHolder);
            

            var unprocessedExtendedLessonsManager = new UnprocessedExtendedLessonsManager(
                groupsHolder,
                lecturersHolder,
                classroomHolder,
                calendarRawItemHolder);

            _lessonProvider = new LessonProvider(unprocessedExtendedLessonsManager, oddDayWeekChecker);
        }

        [Test]
        public async Task SimpleTest()
        {
            var facultyGroups = await groupsHolder.GetFacultiesGroups();
            var facultyGroup = facultyGroups.FirstOrDefault(g => g.Name == "11 І");
            
            //act
            ICollection<Lesson> testLessonsList =
                await _lessonProvider.GetLessonsOnDate(facultyGroup, new DateTime(2019, 10, 2));
            
            //assert
            Assert.AreEqual(4, testLessonsList.Count);
            
            var assert1 = testLessonsList.Where(l => l.LessonNumber == 1).ToList();
            Assert.AreEqual(1, assert1.Count);
            var lesson1 = assert1[0];
            Assert.AreEqual("98", lesson1.Lecturer.TypeId);
            Assert.AreEqual(Fraction.None, lesson1.Fraction);
            Assert.AreEqual(SubGroup.None, lesson1.SubGroup);
            
            var assert2 = testLessonsList.Where(l => l.LessonNumber == 2).ToList();
            Assert.AreEqual(1, assert2.Count);
            var lesson2 = assert2[0];
            Assert.AreEqual("616", lesson2.Lecturer.TypeId);
            Assert.AreEqual(Fraction.None, lesson2.Fraction);
            Assert.AreEqual(SubGroup.None, lesson2.SubGroup);
            
            var assert3 = testLessonsList.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(1, assert3.Count);
            var lesson3 = assert3[0];
            Assert.AreEqual("83", lesson3.Lecturer.TypeId);
            Assert.AreEqual(Fraction.None, lesson3.Fraction);
            Assert.AreEqual(SubGroup.None, lesson3.SubGroup);
            
            
            var assert4 = testLessonsList.Where(l => l.LessonNumber == 4).ToList();
            Assert.AreEqual(1, assert3.Count);
            var lesson4 = assert4[0];
            Assert.AreEqual("1262", lesson4.Lecturer.TypeId);
            Assert.AreEqual(Fraction.None, lesson4.Fraction);
            Assert.AreEqual(SubGroup.None, lesson4.SubGroup);
        }


        [Test]
        public async Task SimpleFractionTest()
        {
            var facultyGroups = await groupsHolder.GetFacultiesGroups();
            var facultyGroup = facultyGroups.FirstOrDefault(g => g.Name == "11 ІПЗ");
            
            //act
            ICollection<Lesson> testLessonsList =
                await _lessonProvider.GetLessonsOnDate(facultyGroup, new DateTime(2019, 9, 30));
            
            //assert
            Assert.AreEqual(2, testLessonsList.Count);
            
            var fractionLesson = testLessonsList.FirstOrDefault(l => l.LessonNumber == 2);
            Assert.AreEqual(Fraction.Numerator, fractionLesson.Fraction);
        }

        [Test]
        public async Task DoubleFractionTest()
        {
            var facultyGroups = await groupsHolder.GetFacultiesGroups();
            var facultyGroup = facultyGroups.FirstOrDefault(g => g.Name == "11 І");
            
            ICollection<Lesson> numeratorLessons =
                await _lessonProvider.GetLessonsOnDate(facultyGroup, new DateTime(2019, 10, 3));
            
            //assert
            Assert.AreEqual(2, numeratorLessons.Count);
            
            Assert.AreEqual(
                Fraction.Numerator,
                numeratorLessons.FirstOrDefault(l => l.LessonNumber == 2).Fraction);
            
            Assert.AreEqual(
                Fraction.Numerator,
                numeratorLessons.FirstOrDefault(l => l.LessonNumber == 3).Fraction);
            
            ICollection<Lesson> denominatorLessons =
                await _lessonProvider.GetLessonsOnDate(facultyGroup, new DateTime(2019, 10, 10));
            
            Assert.AreEqual(1, denominatorLessons.Count);
            
            Assert.AreEqual(
                Fraction.Denominator,
                denominatorLessons.FirstOrDefault(l => l.LessonNumber == 2).Fraction);
        }
    }
}