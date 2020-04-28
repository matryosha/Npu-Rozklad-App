using NpuRozklad.Core.Entities;

namespace NpuRozklad.Telegram
{
    public class TelegramRozkladUser : RozkladUser
    {
        public int TelegramId { get; set; }
        public string Language { get; set; }

        public TelegramRozkladUser()
        {
            
        }

        public TelegramRozkladUser(RozkladUser user)
        :base(user)
        {
            
        }
        public TelegramRozkladUser FillFromRozkladUser(RozkladUser user)
        {
            FacultyGroups.AddRange(user.FacultyGroups);
            return this;
        }
    }
}