using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using NpuTimetableParser;

namespace RozkladNpuAspNetCore.Entities
{
    public class RozkladUser
    {
        public enum LastActionType
        {
            Default,
            WaitForFaculty,
            WaitForGroup,
            Settings,
            SettingsReset,
            WeekSchedule,
            ScheduleMenu
        }
        public RozkladUser()
        {
            this.Guid = System.Guid.NewGuid().ToString();
            this.Groups = new List<Group>();
        }
        [Key]
        public string Guid { get; set; }
        public int TelegramId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacultyShortName { get; set; }
        public int ExternalGroupId { get; set; }
        public bool IsDeleted { get; set; }
        public int QueryCount { get; set; }
        public LastActionType LastAction { get; set; }
        public List<Group> Groups { get; set; }

        [Obsolete("Only for Persistence by EntityFramework")]
        public string GroupsAsJson
        {
            get
            {
                return Groups == null || !Groups.Any()
                    ? null
                    : JsonConvert.SerializeObject(Groups);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Groups.Clear();
                else
                    Groups = JsonConvert.DeserializeObject<List<Group>>(value);
            }
        }
    }
}
