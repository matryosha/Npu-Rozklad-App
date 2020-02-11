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

        public Faculty(string typeId, string shortName, string fullName) : base(typeId)
        {
            if (string.IsNullOrWhiteSpace(shortName)) throw new ArgumentNullException(shortName);
            ShortName = shortName;
            FullName = fullName ?? string.Empty;
        }
    }
}