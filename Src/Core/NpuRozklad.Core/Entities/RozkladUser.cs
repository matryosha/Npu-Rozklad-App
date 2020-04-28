using System.Collections.Generic;

namespace NpuRozklad.Core.Entities
{
    public class RozkladUser
    {
        public RozkladUser()
        { }

        protected RozkladUser(RozkladUser origin)
        {
            Guid = origin.Guid;
            IsDeleted = origin.IsDeleted;
            FacultyGroups = origin.FacultyGroups;
        }
        public string Guid { get; } = System.Guid.NewGuid().ToString();
        public List<Group> FacultyGroups { get; protected set; } = new List<Group>();
        public bool IsDeleted { get; set; }
    }
}