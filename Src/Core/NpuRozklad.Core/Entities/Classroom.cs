using System;

namespace NpuRozklad.Core.Entities
{
    public class Classroom : TypeIdTrait
    {
        public Classroom(string typeId, string name) : base(typeId)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}