using System;

namespace NpuRozklad.Core.Entities
{
    public class Faculty : TypeIdTrait
    {
        public override string TypeId => ShortName;

        public string ShortName { get;  }
        public string FullName { get; }

        public override string ToString()
        {
            return FullName + " " + ShortName;
        }

        public Faculty(string shortName, string fullName) : base(shortName)
        {
            ShortName = shortName;
            FullName = fullName ?? string.Empty;
        }
    }
}