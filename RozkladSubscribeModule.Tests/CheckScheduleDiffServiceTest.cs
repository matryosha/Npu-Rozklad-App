using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NpuTimetableParser;
using RozkladSubscribeModule.Application;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure.Enums;
using RozkladSubscribeModule.Interfaces;
using Xunit;

namespace RozkladSubscribeModule.Tests
{
    public class CheckScheduleDiffServiceTest
    {
        [Fact]
        public async Task AllLessonsSame()
        {
            var lessonsList = new List<Lesson>
            {
                GetLesson(DateTime.Today, 1, 1, Fraction.None, SubGroup.None),
                GetLesson(DateTime.Today, 2, 2, Fraction.Denominator, SubGroup.None),
                GetLesson(DateTime.Today, 3, 3, Fraction.None, SubGroup.First),
                GetLesson(DateTime.Today, 4, 4, Fraction.None, SubGroup.None),
                GetLesson(DateTime.Today, 5, 5, Fraction.None, SubGroup.None),
            };

            var lessonsManager = GetLessonsManager(
                CreateSectionLessons(lessonsList, DateTime.Today),
                CreateSectionLessons(lessonsList, DateTime.Today));

            var checkService = new CheckScheduleDiffService(
                GetLogger(), lessonsManager, GetCheckDatesGiver(new List<DateTime>{DateTime.Today}));

            var checkScheduleResultPayload = await checkService.CheckDiff("asd", 12);

            Assert.False(checkScheduleResultPayload.IsDiff());
            Assert.Empty(checkScheduleResultPayload.UpdatedLessons);
        }

        [Fact]
        public async Task LessonNumber_New_SectionLessons_Has_New_Lesson() {
            var lessonsListLast = new List<Lesson>
            {
                GetLesson(DateTime.Today, 2, 2, Fraction.Denominator, SubGroup.None),
                GetLesson(DateTime.Today, 3, 3, Fraction.None, SubGroup.First),
                GetLesson(DateTime.Today, 4, 4, Fraction.None, SubGroup.None),
                GetLesson(DateTime.Today, 5, 5, Fraction.None, SubGroup.None),
            };

            var lessonsListCurrent = new List<Lesson>(lessonsListLast);
            lessonsListCurrent.Add(GetLesson(DateTime.Today, 10, 1, Fraction.Denominator, SubGroup.None));

            var lessonsManager = GetLessonsManager(
                CreateSectionLessons(lessonsListCurrent, DateTime.Today),
                CreateSectionLessons(lessonsListLast, DateTime.Today));

            var checkService = new CheckScheduleDiffService(
                GetLogger(), lessonsManager, GetCheckDatesGiver(new List<DateTime> { DateTime.Today }));

            var checkScheduleResultPayload = await checkService.CheckDiff("asd", 12);

            Assert.True(checkScheduleResultPayload.IsDiff());
            Assert.NotNull(checkScheduleResultPayload.UpdatedLessons.FirstOrDefault(l =>
                    l.UpdateType == LessonUpdateType.AddedLesson));
            Assert.NotNull(checkScheduleResultPayload.UpdatedLessons.FirstOrDefault(l => l.NewLesson.Subject.Id == 10));
        }

        [Fact]
        public async Task LessonNumber_New_SectionLessons_Removed_Old_Lesson()
        {
            var certainLesson = GetLesson(DateTime.Today, 2, 2, Fraction.Denominator, SubGroup.None);
            var lessonsListLast = new List<Lesson>
            {
                GetLesson(DateTime.Today, 3, 3, Fraction.None, SubGroup.First),
                GetLesson(DateTime.Today, 4, 4, Fraction.None, SubGroup.None),
                GetLesson(DateTime.Today, 5, 5, Fraction.None, SubGroup.None),
            };

            lessonsListLast.Add(certainLesson);

            var lessonsListCurrent = new List<Lesson>(lessonsListLast);
            lessonsListCurrent.Remove(certainLesson);

            var lessonsManager = GetLessonsManager(
                CreateSectionLessons(lessonsListCurrent, DateTime.Today),
                CreateSectionLessons(lessonsListLast, DateTime.Today));

            var checkService = new CheckScheduleDiffService(
                GetLogger(), lessonsManager, GetCheckDatesGiver(new List<DateTime> { DateTime.Today }));

            var checkScheduleResultPayload = await checkService.CheckDiff("asd", 12);

            Assert.True(checkScheduleResultPayload.IsDiff());
            Assert.NotNull(
                checkScheduleResultPayload.UpdatedLessons.FirstOrDefault(
                    l => l.OldLesson.Subject.Id == 2));
            Assert.NotNull(
                checkScheduleResultPayload.UpdatedLessons.FirstOrDefault(
                    l => l.UpdateType == LessonUpdateType.DeletedLesson));
        }

        [Fact]
        public async Task Fraction_New_SectionLessons_Got_New_Fraction_Lessons()
        {
            var oldLessons = new List<Lesson>
            {
                GetLesson(DateTime.Today, 3, 3, Fraction.None, SubGroup.First),
                GetLesson(DateTime.Today, 4, 4, Fraction.None, SubGroup.None),
                GetLesson(DateTime.Today, 5, 5, Fraction.None, SubGroup.None),
            };
            var newLessons = new List<Lesson>
            {
                GetLesson(DateTime.Today, 1, 2, Fraction.Numerator, SubGroup.None),
                GetLesson(DateTime.Today, 2, 2, Fraction.Denominator, SubGroup.None),
            };

            var lessonsManager = GetLessonsManager(
                CreateSectionLessons(newLessons, DateTime.Today),
                CreateSectionLessons(oldLessons, DateTime.Today));

            var checkService = new CheckScheduleDiffService(
                GetLogger(), lessonsManager, GetCheckDatesGiver(new List<DateTime> { DateTime.Today }));

            var checkScheduleResultPayload = await checkService.CheckDiff("asd", 12);

            Assert.True(checkScheduleResultPayload.IsDiff());
            Assert.Equal(2,
                checkScheduleResultPayload.UpdatedLessons.Count(l => l.UpdateType == LessonUpdateType.AddedLesson));
            Assert.NotNull(checkScheduleResultPayload.UpdatedLessons.FirstOrDefault(
                l => l.NewLesson.Subject.Id == 1));
            Assert.NotNull(checkScheduleResultPayload.UpdatedLessons.FirstOrDefault(
                l => l.NewLesson.Subject.Id == 2));
        }

        [Fact]
        public async Task Fraction_New_SectionLessons_Replaced_Old_Fraction_And_Subgroup()
        {
            var oldLessons = new List<Lesson>
            {
                GetLesson(DateTime.Today, 1, 1, Fraction.Numerator, SubGroup.First),
                GetLesson(DateTime.Today, 1, 1, Fraction.Denominator, SubGroup.Second),
                GetLesson(DateTime.Today, 2, 1, Fraction.Denominator, SubGroup.First),
                GetLesson(DateTime.Today, 2, 1, Fraction.Numerator, SubGroup.Second),
            };

            var newLessons = new List<Lesson>
            {
                GetLesson(DateTime.Today, 1, 1, Fraction.Numerator, SubGroup.None),
                GetLesson(DateTime.Today, 2, 1, Fraction.Denominator, SubGroup.None)
            };

            var lessonsManager = GetLessonsManager(
                CreateSectionLessons(newLessons, DateTime.Today),
                CreateSectionLessons(oldLessons, DateTime.Today));

            var checkService = new CheckScheduleDiffService(
                GetLogger(), lessonsManager, GetCheckDatesGiver(new List<DateTime> { DateTime.Today }));

            var checkScheduleResultPayload = await checkService.CheckDiff("asd", 12);

            Assert.True(checkScheduleResultPayload.IsDiff());
        }

        private SectionLessons CreateSectionLessons(List<Lesson> lessons, DateTime lessonsDate)
        {
            var result =  new SectionLessons("idk", 1337);

            result.Lessons.Add(lessonsDate, lessons);
            return result;
        }

        private IDateTimesForScheduleDiffCheckGiver GetCheckDatesGiver(
            List<DateTime> dateTimes)
        {
            return Mock.Of<IDateTimesForScheduleDiffCheckGiver>(
                s => s.GetDates() == dateTimes);
        }

        private ISectionLessonsManager GetLessonsManager(
            SectionLessons currentSection,
            SectionLessons lastSection)
        {

            var mock = new Mock<ISectionLessonsManager>();
            mock.Setup(m => m.GetCurrentSectionLessons(
                    It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(currentSection);
            mock.Setup(m => m.GetLastSectionLessons(
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SectionLessons>()))
                .Returns(lastSection);

            return mock.Object;
        }

        private ILogger<CheckScheduleDiffService> GetLogger()
        {
            return Mock.Of<ILogger<CheckScheduleDiffService>>();
        }

        private Lesson GetLesson(
            DateTime lessonDate,
            int lessonSubjectId,
            int lessonNumber,
            Fraction fraction,
            SubGroup subGroup)
        {
            return new Lesson
            {
                Subject = new Subject {Id = lessonSubjectId},
                LessonNumber = lessonNumber,
                LessonDate = lessonDate,
                Fraction = fraction,
                SubGroup = subGroup
            };
        } 
    }


}
