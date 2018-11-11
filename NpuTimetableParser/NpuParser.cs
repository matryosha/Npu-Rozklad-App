using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace NpuTimetableParser
{
    public class NpuParser
    {
        private readonly RestClient _client;
        //private NpuParserInstance _testInstance;
        private readonly NpuParserHelper _helper;
        private readonly Dictionary<string, NpuParserInstance> _npuInstances;
        private List<Faculty> _faculties;

        public NpuParser()
        {
            _client = new RestClient("http://ei.npu.edu.ua");
            //_testInstance = new NpuParserInstance();
            _helper = new NpuParserHelper(_client, new RawStringParser());
            _npuInstances = new Dictionary<string, NpuParserInstance>();
            _faculties = _helper.GetFaculties();
        }

        public List<Faculty> GetFaculties() => _faculties ?? (_faculties = _helper.GetFaculties());

        public Task<List<Group>> GetGroups(string facultyShortName)
        {
            if (!_npuInstances.ContainsKey(facultyShortName))
            {
                if (_faculties.Any(f => f.ShortName == facultyShortName))
                {
                    var npuInstance = new NpuParserInstance(_client, facultyShortName);
                    _npuInstances.Add(facultyShortName, npuInstance);
                }
                else
                {
                    return null;
                }
            }

            return _npuInstances[facultyShortName].GetGroups();
        }

        public Task<List<Lesson>> GetLessonsOnDate(string facultyShortName, string groupShortName, DateTime date)
        {
            if (!_npuInstances.ContainsKey(facultyShortName))
            {
                if (_faculties.Any(f => f.ShortName == facultyShortName))
                {
                    var npuInstance = new NpuParserInstance(_client, facultyShortName);
                    _npuInstances.Add(facultyShortName, npuInstance);
                }
                else
                {
                    return null;
                }
            }

            var groups = _npuInstances[facultyShortName].GetGroups().Result;
            var groupId = groups.FirstOrDefault(g => g.ShortName == groupShortName).ExternalId;
            return _npuInstances[facultyShortName].GetLessonsOnDate(date, groupId);
        }
    }
}
