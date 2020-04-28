namespace NpuRozklad.Core
{
    public class LocalizationValue
    {
        public string Value { get; set; }
        public LocalizationValue(string value)
        {
            Value = value;
        }
        public override string ToString() => Value;

        public static implicit operator string(LocalizationValue input)
        {
            return input.Value;
        }
    }
}
