namespace RozkladSubscribeModule.Entities
{
    internal class SubscribedUser
    {
        public SubscribedUser(
            int telegramId, 
            int groupExternalId, 
            string facultyShortName)
        {
            TelegramId = telegramId;
            GroupExternalId = groupExternalId;
            FacultyShortName = facultyShortName;
        }

        public int TelegramId { get; }
        public int GroupExternalId { get; }
        public string FacultyShortName { get; }

        public override int GetHashCode()
        {
            return TelegramId.GetHashCode() + GroupExternalId.GetHashCode() + FacultyShortName.GetHashCode();
        }
    }
}
