namespace RozkladSubscribeModule.Entities
{
    internal class SubscribedUser
    {
        public SubscribedUser(
            int telegramId, 
            int groupExternalId,
            long chatId,
            string facultyShortName,
            string groupShortName)
        {
            ChatId = chatId;
            TelegramId = telegramId;
            GroupExternalId = groupExternalId;
            FacultyShortName = facultyShortName;
            GroupShortName = groupShortName;
        }

        public int TelegramId { get; }
        public int GroupExternalId { get; }
        public string FacultyShortName { get; }
        public long ChatId { get; }
        public string GroupShortName { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if(obj.GetType() != typeof(SubscribedUser))
                return false;

            return GetHashCode() == ((SubscribedUser) obj).GetHashCode();
        }

        public override int GetHashCode()
        {
            return TelegramId.GetHashCode() +
                   GroupExternalId.GetHashCode() +
                   FacultyShortName.GetHashCode() +
                   ChatId.GetHashCode() +
                   GroupShortName.GetHashCode();
        }

        public override string ToString()
        {
            return $"{TelegramId}|{FacultyShortName}|{GroupExternalId}|{ChatId}|{GroupShortName}";
        }
    }
}
