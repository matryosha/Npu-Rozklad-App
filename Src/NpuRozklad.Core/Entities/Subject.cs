namespace NpuRozklad.Core.Entities
{
    public class Subject : TypeIdTrait
    {
        public string Name { get; }
        
        public override string ToString()
        {
            return Name;
        }

        public Subject(string name) : base(name)
        {
            Name = name;
        }
    }
}