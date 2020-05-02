using System.Collections.Generic;

namespace NpuRozklad.Core.Entities
{
    public class RozkladUser
    {
        public RozkladUser(string guid = null)
        {
            Guid = string.IsNullOrWhiteSpace(guid) ? System.Guid.NewGuid().ToString() : guid;
        }

        protected RozkladUser(RozkladUser origin)
        {
            Guid = origin.Guid;
            IsDeleted = origin.IsDeleted;
            FacultyGroups = origin.FacultyGroups;
        }
        public string Guid { get; private set; }
        public List<Group> FacultyGroups { get; protected set; } = new List<Group>();
        public bool IsDeleted { get; set; }
    }
}