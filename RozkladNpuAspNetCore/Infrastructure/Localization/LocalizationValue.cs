namespace RozkladNpuAspNetCore.Infrastructure.Localization
{
    public class LocalizationValue
    {
        public string Value { get; set; }
        
        public override string ToString() => Value;

        public static implicit operator string(LocalizationValue input)
        {
            return input.Value;
        }
    }
}
