using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RozkladNpuAspNetCore.Models
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
            WeekSchedule
        }
        public RozkladUser()
        {
            this.Guid = System.Guid.NewGuid().ToString();
        }
        [Key]
        public string Guid { get; set; }
        public int TelegramId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacultyShortName { get; set; }
        public int ExternalGroupId { get; set; }
        public LastActionType LastAction { get; set; }
    }
}
