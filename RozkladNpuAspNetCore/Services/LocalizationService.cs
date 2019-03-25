using System;
using RozkladNpuAspNetCore.Interfaces;

namespace RozkladNpuAspNetCore.Services
{
    public class LocalizationService : ILocalizationService
    {
        public string this[string language, string text]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
