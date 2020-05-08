using NpuRozklad.Core.Entities;

namespace NpuRozklad.Telegram
{
    public class TelegramRozkladUser : RozkladUser
    {
        public int TelegramId { get; set; }
        public string Language { get; set; }

        public TelegramRozkladUser(string guid = null) : base(guid)
        { }
        
        public TelegramRozkladUser FillFromRozkladUser(RozkladUser user)
        {
            FacultyGroups = user.FacultyGroups;
            return this;
        }

        public override string ToString()
        {
            return $"Telegram id: {TelegramId}. {base.ToString()}";
        }
    }
}