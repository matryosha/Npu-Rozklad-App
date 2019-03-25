namespace RozkladNpuAspNetCore.Interfaces
{
    interface ILocalizationService
    {
        string this[string language, string text] { get; set; }
    }
}
