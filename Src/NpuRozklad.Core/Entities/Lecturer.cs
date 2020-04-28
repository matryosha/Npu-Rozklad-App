namespace NpuRozklad.Core.Entities
{
    public class Lecturer : TypeIdTrait
    {
        public string FullName { get; }

        public override string ToString()
        {
            return FullName;
        }

        public Lecturer(string typeId, string fullName) : base(typeId)
        {
            FullName = fullName ?? string.Empty;
        }
    }
}