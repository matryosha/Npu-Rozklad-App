using System.ComponentModel.DataAnnotations;

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
        public bool IsDeleted { get; set; }
        public int QueryCount { get; set; }
        public LastActionType LastAction { get; set; }
    }
}
