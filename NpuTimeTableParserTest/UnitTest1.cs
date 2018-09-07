using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpuTimetableParser;

namespace NpuTimeTableParserTest
{
    [TestClass]
    public class NpuParserTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            NpuParser parser = new NpuParser();
            var collection = parser.FillCalendarRawList();
            
        }
    }
}
