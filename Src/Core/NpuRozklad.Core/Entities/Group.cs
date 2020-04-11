using System;

namespace NpuRozklad.Core.Entities
{
    public class Group : TypeIdTrait
    {
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public Group(string typeId, string name) : base(typeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Group name cannot be null or empty");
            Name = name;
        }
    }
}