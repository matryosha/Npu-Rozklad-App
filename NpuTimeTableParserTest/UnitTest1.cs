using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpuTimetableParser;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;

namespace NpuTimeTableParserTest
{
    [TestClass]
    public class NpuParserTest
    {
        [TestMethod]
        public async Task TestPolygon()
        {
            var mockClient = new MockRestClient();
            mockClient.CalendarRawContent = ReadMockContent("CalendarRawContent.txt");
            //mockClient.CalendarRawContent = ReadMockContent("testcalendar.txt"); 
            mockClient.GroupsRawContent = ReadMockContent("GroupsRawContent.txt");
            mockClient.LecturesRawContent = ReadMockContent("LecturesRawContent.txt");
            mockClient.ClassroomsRawContent = ReadMockContent("ClassroomsRawContent.txt");
            mockClient.FacultiesRawContent = ReadMockContent("FacultiesRawContent.txt");
            NpuParser parser = new NpuParser(mockClient);
            var faculties = await parser.GetFaculties();
            //var lessons = await parser.GetLessonsOnDate(new DateTime(2017, 9, 8), 87);
            var lessons = await parser.GetLessonsOnDate(new DateTime(2017, 11, 17), 87);
            var count = lessons.Count;

        }

        [TestMethod]
        public async Task GetLessonsOnDateTest1()
        {
            //arrange
            var mockClient = new MockRestClient();
            mockClient.CalendarRawContent = ReadMockContent("CalendarRawContent.txt");
            mockClient.GroupsRawContent = ReadMockContent("GroupsRawContent.txt");
            mockClient.LecturesRawContent = ReadMockContent("LecturesRawContent.txt");
            mockClient.ClassroomsRawContent = ReadMockContent("ClassroomsRawContent.txt");
            NpuParser parser = new NpuParser(mockClient);

            //act
            List<Lesson> testLessonsList = await parser.GetLessonsOnDate(new DateTime(2017, 11, 17), 87);

            //assert
            Assert.AreEqual(4, testLessonsList.Count);

            var assert1 = testLessonsList.Where(l => l.LessonNumber == 2).ToList();
            Assert.AreEqual(1, assert1.Count);
            Assert.AreEqual(111, assert1[0].Lecturer.ExternalId);

            var assert2 = testLessonsList.Where(l => l.LessonNumber == 1).ToList();
            Assert.AreEqual(1, assert2.Count);
            Assert.AreEqual(83, assert2[0].Lecturer.ExternalId);

            var assert3 = testLessonsList.Where(l => l.LessonNumber == 3).ToList();
            Assert.AreEqual(2, assert3.Count);
            Assert.AreEqual(true, assert3.Any( l => l.Subject.Name =="Комп'ютерна дискретна математика"));
            Assert.AreEqual(true, assert3.Any( l => l.Subject.Name == "Фізика"));
        }
        [TestMethod]
        public void ReplaceLessonTest()
        {
            var newLesson = new Lesson() {Subject = new Subject() {Name = "new"}};
            var oldLesson = new Lesson() {Subject = new Subject() {Name = "old"}};
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
            Assert.AreEqual("new2_0_1", assertList.FirstOrDefault( l => l.SubGroup == SubGroup.First).Subject.Name);
            Assert.AreEqual("new2_0_2", assertList.FirstOrDefault( l => l.SubGroup == SubGroup.Second).Subject.Name);
        }

        public async Task NpuParser_Example()
        {
            //Create instance
            var npuParser = new NpuParser();
            //Get all faculties
            var faculties = await npuParser.GetFaculties();
            //Select certain faculty
            var fi = faculties.First(f => f.ShortName == "fi");
            //Set faculty sending faculty object
            npuParser.SetFaculty(fi);
            //Or just using short name
            npuParser.SetFaculty(fi.ShortName);
            //Get faculty's groups
            var groups = await npuParser.GetGroups();
            //Get lesson list by passing date and group id
            var lessons = await npuParser.GetLessonsOnDate(new DateTime(2018, 10, 29), groups[1].ExternalId);
        }
        public string ReadMockContent(string fileName)
        {
            return File.ReadAllText($"{fileName}");
        }
    }



    public class MockRestClient : IRestClient
    {
        public string CalendarRawContent { get; set; }
        public string GroupsRawContent { get; set; }
        public string LecturesRawContent { get; set; }
        public string ClassroomsRawContent { get; set; }
        public string FacultiesRawContent { get; set; }

        public IRestResponse Execute(IRestRequest request)
        {
            var codeParameter = request.Parameters.First(a => (string) a.Name == "code");

            switch (codeParameter.Value.ToString())
            {
                case "get calendar":
                    return new MockRestResponse(CalendarRawContent);
                case "get groups":
                    return new MockRestResponse(GroupsRawContent);
                case "get lectors":
                    return new MockRestResponse(LecturesRawContent);
                case "get auditories":
                    return new MockRestResponse(ClassroomsRawContent);
                case "get faculties":
                    return new MockRestResponse(FacultiesRawContent);
            }
            throw new Exception("There is no such a code");
        }

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback)
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> Deserialize<T>(IRestResponse response)
        {
            throw new NotImplementedException();
        }



        public IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            throw new NotImplementedException();
        }

        public byte[] DownloadData(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public byte[] DownloadData(IRestRequest request, bool throwOnError)
        {
            throw new NotImplementedException();
        }

        public Uri BuildUri(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncGet(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncGet<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public void ConfigureWebRequest(Action<HttpWebRequest> configurator)
        {
            throw new NotImplementedException();
        }

        public void AddHandler(string contentType, IDeserializer deserializer)
        {
            throw new NotImplementedException();
        }

        public void RemoveHandler(string contentType)
        {
            throw new NotImplementedException();
        }

        public void ClearHandlers()
        {
            throw new NotImplementedException();
        }

        public IRestResponse ExecuteAsGet(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod) where T : new()
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod) where T : new()
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public CookieContainer CookieContainer { get; set; }
        public bool AutomaticDecompression { get; set; }
        public int? MaxRedirects { get; set; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
        public int ReadWriteTimeout { get; set; }
        public bool UseSynchronizationContext { get; set; }
        public IAuthenticator Authenticator { get; set; }
        public Uri BaseUrl { get; set; }
        public Encoding Encoding { get; set; }
        public string ConnectionGroupName { get; set; }
        public bool PreAuthenticate { get; set; }
        public bool UnsafeAuthenticatedConnectionSharing { get; set; }
        public IList<Parameter> DefaultParameters { get; }
        public string BaseHost { get; set; }
        public bool AllowMultipleDefaultParametersWithSameName { get; set; }
        public X509CertificateCollection ClientCertificates { get; set; }
        public IWebProxy Proxy { get; set; }
        public RequestCachePolicy CachePolicy { get; set; }
        public bool Pipelined { get; set; }
        public bool FollowRedirects { get; set; }
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }
    }

    public class MockRestResponse : IRestResponse
    {
        public MockRestResponse(string mockResponse)
        {
            Content = mockResponse;
            ContentLength = 1;

        }
        public IRestRequest Request { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string ContentEncoding { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessful { get; }
        public string StatusDescription { get; set; }
        public byte[] RawBytes { get; set; }
        public Uri ResponseUri { get; set; }
        public string Server { get; set; }
        public IList<RestResponseCookie> Cookies { get; }
        public IList<Parameter> Headers { get; }
        public ResponseStatus ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
        public Version ProtocolVersion { get; set; }
    }
}
