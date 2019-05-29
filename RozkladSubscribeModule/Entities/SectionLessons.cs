namespace RozkladSubscribeModule.Entities
{
    internal class SectionLessons
    {
        public SectionLessons(string facultyShortName, int groupExternalId)
        {
            FacultyShortName = facultyShortName;
            GroupExternalId = groupExternalId;
        }

        public string FacultyShortName { get; }
        public int GroupExternalId { get; }
    }
}