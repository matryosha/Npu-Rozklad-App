using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace NpuTimetableParser
{
    public class NpuParser
    {
        private IRestClient _client;
        private ReaderWriterLockSlim _groupsLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim _lessonsLock = new ReaderWriterLockSlim();

        private RawStringParser _parser;
        //private NpuParserInstance _testInstance;
        private readonly NpuParserHelper _helper;
        private readonly Dictionary<string, NpuParserInstance> _npuInstances;
        private List<Faculty> _faculties;

        public NpuParser()
        {
            _client = new RestClient("http://ei.npu.edu.ua");
            _parser = new RawStringParser();
            _helper = new NpuParserHelper(_client, _parser);
            _npuInstances = new Dictionary<string, NpuParserInstance>();
            _faculties = _helper.GetFaculties();
        }

        public List<Faculty> GetFaculties() => _faculties;

        public Task<List<Group>> GetGroups(string facultyShortName)
        {
            if (!_npuInstances.ContainsKey(facultyShortName))
            {
                if (_faculties.Any(f => f.ShortName == facultyShortName))
                {
                    _groupsLock.EnterWriteLock();
                    if (!_npuInstances.ContainsKey(facultyShortName))
                    {
                        var npuInstance = new NpuParserInstance(_client, _parser, facultyShortName);
                        _npuInstances.Add(facultyShortName, npuInstance);
                    }
                    _groupsLock.ExitWriteLock();
                }
                else
                {
                    return null;
                }
            }

            return _npuInstances[facultyShortName].GetGroups();
        }

        public async Task<List<Lesson>> GetLessonsOnDate(string facultyShortName, int groupId, DateTime date)
        {
            if (!_npuInstances.ContainsKey(facultyShortName))
            {
                if (_faculties.Any(f => f.ShortName == facultyShortName))
                {
                    _lessonsLock.EnterWriteLock();
                    if(!_npuInstances.ContainsKey(facultyShortName))
                    {
                        var npuInstance = new NpuParserInstance(_client, _parser,facultyShortName);
                        _npuInstances.Add(facultyShortName, npuInstance);
                    }
                    _lessonsLock.ExitWriteLock();
                }
                else
                {
                    return null;
                }
            }

            return await _npuInstances[facultyShortName].GetLessonsOnDate(date, groupId);
        }

        public void SetClient(IRestClient client)
        {
            _client = client;
        }
    }
}
