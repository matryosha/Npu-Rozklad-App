namespace RozkladNpuAspNetCore.Interfaces
{
    public interface ILocalizationService
    {
        string this[string language, string text] { get; set; }
    }
}
