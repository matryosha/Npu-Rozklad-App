namespace NpuRozklad.Core.Entities
{
    public class Group : TypeIdTrait
    {
        public string FullName { get; }
        public string ShortName { get; }

        public override string ToString()
        {
            return ShortName;
        }

        public Group(string typeId, string fullName, string shortName) : base(typeId)
        {
            ShortName = shortName ?? string.Empty;
            FullName = fullName ?? string.Empty;
        }
    }
}