using System.Collections.Generic;
using System.Linq;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Persistence
{
    internal class RozkladUserWrapper : RozkladUser
    {
        public RozkladUserWrapper(string guid = null)
            : base(guid)
        {
        }

        public RozkladUserWrapper(RozkladUser rozkladUser)
            : base(rozkladUser)
        {
            FacultyGroupsTypeIds = FacultyGroups.Select(g => g.TypeId).ToList();
        }

        public List<string> FacultyGroupsTypeIds { get; set; }
    }
}