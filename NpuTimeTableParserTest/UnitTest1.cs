using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpuTimetableParser;
using NpuTimeTableParserTest.Infrastructure;

namespace NpuTimeTableParserTest
{
    [TestClass]
    public class NpuParserTest
    {
        [TestMethod]
        public async Task SimpleLessonsTest()
        {
            //arrange
            var parser = GetParser();
            var groups = await parser.GetGroups("fi");

            //act
            List<Lesson> testLessonsList = 
                await parser.GetLessonsOnDate("fi", 75, new DateTime(2019, 3, 18));

            //assert
            Assert.AreEqual(3, testLessonsList.Count);

            var assert1 = testLessonsList.Where(l => l.LessonNumber == 2).ToList();
            Assert.AreEqual(1, assert1.Count);
            var lesson1 = assert1[0];
            Assert.AreEqual(389, lesson1.Lecturer.ExternalId);
            Assert.AreEqual(Fraction.None, lesson1.Fraction);
            Assert.AreEqual(SubGroup.None, lesson1.SubGroup);

            var assert2 = testLessonsList.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(1, assert2.Count);
            var lesson2 = assert2[0];
            Assert.AreEqual(389, lesson2.Lecturer.ExternalId);
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
            List<Lesson> lessons = 
                await parser.GetLessonsOnDate("fi", 75, new DateTime(2019, 3, 21));

            Assert.AreEqual(2, lessons.Count);
            var fractionLesson = lessons.FirstOrDefault(l => l.LessonNumber == 3);
            Assert.AreEqual(Fraction.Denominator, fractionLesson.Fraction);
        }

        [TestMethod]
        public async Task DoubleFractionTest()
        {
            var parser = GetParser();
            List<Lesson> lessons =
                await parser.GetLessonsOnDate("fi", 86, new DateTime(2019, 3, 19));

            Assert.AreEqual(2, lessons.Count);
            var classes3 = lessons.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(1, classes3.Count);
            var class3 = classes3.FirstOrDefault();
            Assert.AreEqual(Fraction.Denominator, class3.Fraction);
            Assert.AreEqual("Педагогіка", class3.Subject.Name);
        }

        [TestMethod]
        public async Task SubGroupTest()
        {
            var parser = GetParser();
            List<Lesson> lessons =
                await parser.GetLessonsOnDate("fi", 490, new DateTime(2019, 3, 20));

            Assert.AreEqual(4, lessons.Count);
            var subGroupLessons = lessons.Where(l => l.LessonNumber == 4).ToList();
            Assert.AreEqual(2, subGroupLessons.Count);
            var firstGroupLesson = subGroupLessons.FirstOrDefault(l => l.SubGroup == SubGroup.First);
            var secondGroupLesson = subGroupLessons.FirstOrDefault(l => l.SubGroup == SubGroup.Second);
            Assert.AreEqual("Економіка та бізнес (за вибором)", firstGroupLesson.Subject.Name);
            Assert.AreEqual("Політологія", secondGroupLesson.Subject.Name);
        }

        public static string ReadMockContent(string fileName)
        {
            return File.ReadAllText($"{fileName}");
        }

        private static NpuParser GetParser()
        {
            var mockClient = new MockRestClient
            {
                CalendarRawContent = ReadMockContent("CalendarRawContent.txt"),
                GroupsRawContent = ReadMockContent("GroupsRawContent.txt"),
                LecturesRawContent = ReadMockContent("LecturesRawContent.txt"),
                ClassroomsRawContent = ReadMockContent("ClassroomsRawContent.txt")
            };
            var parser = new NpuParser();
            parser.SetClient(mockClient);
            return parser;
        }
    }

    [TestClass]
    public class NpuParserHelperTests
    {
        [TestMethod]
        public void ReplaceLessonTest()
        {
            var newLesson = new Lesson() { Subject = new Subject() { Name = "new" } };
            var oldLesson = new Lesson() { Subject = new Subject() { Name = "old" } };
            var list = new List<Lesson>();

            list.Add(oldLesson);
            NpuParserHelper.ReplaceLesson(list, newLesson, oldLesson);

            Assert.AreEqual("new", list[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_AllFractionTest()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old"
                },
                Fraction = Fraction.None,
                LessonNumber = 1
            };
            var newLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new"
                },
                Fraction = Fraction.None,
                LessonNumber = 1
            };

            resultLessonsList.Add(oldLessonFractionNone);
            newLessonsList.Add(newLessonFractionNone);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            Assert.AreEqual("new", resultLessonsList[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_OldDenominator_NewNoneTest()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLessonFractionDenominator = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old"
                },
                Fraction = Fraction.Denominator,
                LessonNumber = 1
            };
            var newLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new"
                },
                Fraction = Fraction.None,
                LessonNumber = 1
            };

            resultLessonsList.Add(oldLessonFractionDenominator);
            newLessonsList.Add(newLessonFractionNone);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            Assert.AreEqual("new", resultLessonsList[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_OldNumerator_NewNoneTest()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLessonFractionNumerator = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old"
                },
                Fraction = Fraction.Numerator,
                LessonNumber = 1
            };
            var newLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new"
                },
                Fraction = Fraction.None,
                LessonNumber = 1
            };

            resultLessonsList.Add(oldLessonFractionNumerator);
            newLessonsList.Add(newLessonFractionNone);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            Assert.AreEqual("new", resultLessonsList[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_OldSubGroup_NewNoneTest()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLessonFractionSubgroup = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old"
                },
                SubGroup = SubGroup.First,
                LessonNumber = 1
            };
            var newLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new"
                },
                SubGroup = SubGroup.None,
                LessonNumber = 1
            };

            resultLessonsList.Add(oldLessonFractionSubgroup);
            newLessonsList.Add(newLessonFractionNone);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            Assert.AreEqual("new", resultLessonsList[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_OldSubGroupFirst_NewSubgroupFirstTest()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLessonFractionSubgroup = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old"
                },
                SubGroup = SubGroup.First,
                LessonNumber = 1
            };
            var newLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new"
                },
                SubGroup = SubGroup.First,
                LessonNumber = 1
            };

            resultLessonsList.Add(oldLessonFractionSubgroup);
            newLessonsList.Add(newLessonFractionNone);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            Assert.AreEqual("new", resultLessonsList[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_OldSubGroupAndFractionSet_NewSubGroupAndFractionNoneTest()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLessonFractionSubgroup = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old"
                },
                Fraction = Fraction.Numerator,
                SubGroup = SubGroup.First,
                LessonNumber = 1
            };
            var newLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new"
                },
                Fraction = Fraction.None,
                SubGroup = SubGroup.None,
                LessonNumber = 1
            };

            resultLessonsList.Add(oldLessonFractionSubgroup);
            newLessonsList.Add(newLessonFractionNone);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            Assert.AreEqual("new", resultLessonsList[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_MultipleLessons_Test()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLesson1 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old1"
                },
                LessonNumber = 1
            };
            var oldLesson2 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old2"
                },
                LessonNumber = 2
            };
            var oldLesson3 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old3"
                },
                LessonNumber = 3
            };

            var newLesson1 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new1"
                },
                LessonNumber = 1
            };

            var newLesson3 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new3"
                },
                LessonNumber = 3
            };

            resultLessonsList.Add(oldLesson1);
            resultLessonsList.Add(oldLesson2);
            resultLessonsList.Add(oldLesson3);
            newLessonsList.Add(newLesson1);
            newLessonsList.Add(newLesson3);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert
            var assert1 = resultLessonsList.Where(l => l.LessonNumber == 1).ToList();
            Assert.AreEqual(1, assert1.Count);
            Assert.AreEqual("new1", assert1[0].Subject.Name);

            var assert2 = resultLessonsList.Where(l => l.LessonNumber == 2).ToList();
            Assert.AreEqual(1, assert1.Count);
            Assert.AreEqual("old2", assert2[0].Subject.Name);

            var assert3 = resultLessonsList.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(1, assert1.Count);
            Assert.AreEqual("new3", assert3[0].Subject.Name);

        }

        [TestMethod]
        public void MergeLessonsList_MultipleLessonsFraction_Test()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLesson1 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old1_1"
                },
                Fraction = Fraction.Numerator,
                LessonNumber = 1
            };
            var oldLesson2 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old1_2"
                },
                Fraction = Fraction.Denominator,
                LessonNumber = 1
            };
            var oldLesson3 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old2_1_1"
                },
                SubGroup = SubGroup.First,
                Fraction = Fraction.Numerator,
                LessonNumber = 2
            };
            var oldLesson4 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old2_1_2"
                },
                Fraction = Fraction.Numerator,
                SubGroup = SubGroup.Second,
                LessonNumber = 2
            };
            var oldLesson5 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old3_1_2"
                },
                Fraction = Fraction.Numerator,
                SubGroup = SubGroup.Second,
                LessonNumber = 3
            };

            var oldLesson6 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old3_2_1"
                },
                Fraction = Fraction.Denominator,
                SubGroup = SubGroup.First,
                LessonNumber = 3
            };

            var newLesson1 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new1"
                },
                Fraction = Fraction.None,
                LessonNumber = 1
            };

            var newLesson2 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new2"
                },
                Fraction = Fraction.Numerator,
                LessonNumber = 2
            };

            var newLesson3 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new3"
                },
                Fraction = Fraction.None,
                LessonNumber = 3
            };

            resultLessonsList.Add(oldLesson1);
            resultLessonsList.Add(oldLesson2);
            resultLessonsList.Add(oldLesson3);
            resultLessonsList.Add(oldLesson4);
            resultLessonsList.Add(oldLesson5);
            resultLessonsList.Add(oldLesson6);
            newLessonsList.Add(newLesson1);
            newLessonsList.Add(newLesson2);
            newLessonsList.Add(newLesson3);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert
            var assertList1 = resultLessonsList.Where(l => l.LessonNumber == 1).ToList();
            Assert.AreEqual(1, assertList1.Count);
            Assert.AreEqual("new1", assertList1[0].Subject.Name);

            var assertList2 = resultLessonsList.Where(l => l.LessonNumber == 2).ToList();
            Assert.AreEqual(1, assertList2.Count);
            Assert.AreEqual("new2", assertList2[0].Subject.Name);

            var assertList3 = resultLessonsList.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(1, assertList3.Count);
            Assert.AreEqual("new3", assertList3[0].Subject.Name);
        }

        [TestMethod]
        public void MergeLessonsList_AddLesson_Test()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLesson1 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old1"
                },
                LessonNumber = 1
            };
            var oldLesson2 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old2"
                },
                LessonNumber = 2
            };
            var oldLesson3 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old3"
                },
                LessonNumber = 3
            };
            var newLessonFractionNone = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new4"
                },
                SubGroup = SubGroup.None,
                LessonNumber = 4
            };

            resultLessonsList.Add(oldLesson1);
            resultLessonsList.Add(oldLesson2);
            resultLessonsList.Add(oldLesson3);
            newLessonsList.Add(newLessonFractionNone);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            var assertList = resultLessonsList.Where(l => l.LessonNumber == 4).ToList();
            Assert.AreEqual(1, assertList.Count);
            Assert.AreEqual("new4", assertList[0].Subject.Name);
        }
        /// <summary>
        /// Test case when trying to add 2 new lesson with the same lesson number but different group when early this lesson number was empty
        /// </summary>
        [TestMethod]
        public void MergeLessonsList_NoOldLesson_2newSubGroupLessons_Test()
        {
            //arrange
            var resultLessonsList = new List<Lesson>();
            var newLessonsList = new List<Lesson>();
            var oldLesson1 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "old1"
                },
                LessonNumber = 1
            };

            var newLesson1 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new2_0_1"
                },
                SubGroup = SubGroup.First,
                LessonNumber = 2
            };

            var newLesson2 = new Lesson()
            {
                Subject = new Subject()
                {
                    Name = "new2_0_2"
                },
                SubGroup = SubGroup.Second,
                LessonNumber = 2
            };

            resultLessonsList.Add(oldLesson1);
            newLessonsList.Add(newLesson1);
            newLessonsList.Add(newLesson2);

            //act
            NpuParserHelper.MergeLessonsList(resultLessonsList, newLessonsList);

            //assert

            var assertList1 = resultLessonsList.Where(l => l.LessonNumber == 1).ToList();
            Assert.AreEqual(1, assertList1.Count);
            Assert.AreEqual("old1", assertList1[0].Subject.Name);

            var assertList = resultLessonsList.Where(l => l.LessonNumber == 2).ToList();
            Assert.AreEqual(2, assertList.Count);
            Assert.AreEqual("new2_0_1", assertList.FirstOrDefault(l => l.SubGroup == SubGroup.First).Subject.Name);
            Assert.AreEqual("new2_0_2", assertList.FirstOrDefault(l => l.SubGroup == SubGroup.Second).Subject.Name);
        }
    }
}
