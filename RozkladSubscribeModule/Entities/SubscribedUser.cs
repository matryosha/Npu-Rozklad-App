namespace RozkladSubscribeModule.Entities
{
    internal class SubscribedUser
    {
        public SubscribedUser(
            int telegramId, 
            int groupExternalId,
            long chatId,
            string facultyShortName)
        {
            ChatId = chatId;
            TelegramId = telegramId;
            GroupExternalId = groupExternalId;
            FacultyShortName = facultyShortName;
        }

        public int TelegramId { get; }
        public int GroupExternalId { get; }
        public string FacultyShortName { get; }
        public long ChatId { get; }

        public override int GetHashCode()
        {
            return TelegramId.GetHashCode() +
                   GroupExternalId.GetHashCode() +
                   FacultyShortName.GetHashCode() +
                   ChatId.GetHashCode();
        }
    }
}
