using System;

namespace NpuRozklad.Core.Entities
{
    public class Group : TypeIdTrait
    {
        public string Name { get; }
        public Faculty Faculty { get; }

        public override string ToString()
        {
            return Name;
        }

        public Group(string typeId, string name, Faculty faculty) : base(typeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Group name cannot be null or empty");
            Faculty = faculty ?? throw new ArgumentNullException(nameof(faculty), "Faculty cannot be null");
            Name = name;
        }
    }
}