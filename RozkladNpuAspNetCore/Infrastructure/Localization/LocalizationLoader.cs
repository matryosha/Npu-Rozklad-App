using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace RozkladNpuAspNetCore.Infrastructure.Localization
{
    public static class LocalizationLoader
    {
        public static List<RozkladLocalization> LoadAll()
        {
            var result = new List<RozkladLocalization>();
            var localizationFilesPaths = GetLocalizationFilesPaths();
            foreach (var path in localizationFilesPaths)
            {
                result.Add(
                    JsonConvert.DeserializeObject<RozkladLocalization>(
                        File.ReadAllText(path)));
            }

            return result;
        }

        private static string[] GetLocalizationFilesPaths()
        {
            var localizationDirPath = GetLocalizationDirectory();
            var result = Directory.GetFiles(localizationDirPath);
            if (!result.Any())
                throw new Exception("There is not any localization file");
            return result;
        }

        private static string GetLocalizationDirectory()
        {
            var result = Path.Combine(Directory.GetCurrentDirectory(), "Properties/localizations/");
            if (!Directory.Exists(result))
                throw new Exception("Localizations directory doesn't exist");
            return result;
        }
    }
}
