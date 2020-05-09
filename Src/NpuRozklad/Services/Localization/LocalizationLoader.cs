using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Services.Localization
{
    public class LocalizationLoader
    {
        private readonly IAppWorkingDirectory _appWorkingDirectory;
        private string _localizationFilesPath;

        public LocalizationLoader(LocalizationLoaderOptions options, IAppWorkingDirectory appWorkingDirectory)
        {
            _appWorkingDirectory = appWorkingDirectory;
            Init(options);
        }

        public Dictionary<string, RozkladLocalization> LoadAll()
        {
            var localizationFilesPaths = GetPathToLocalizationFiles();

            return localizationFilesPaths.Select(path =>
                    JsonConvert.DeserializeObject<RozkladLocalization>(File.ReadAllText(path)))
                .ToDictionary(localizationValue => localizationValue.ShortName);
        }

        private void Init(LocalizationLoaderOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.PathToLocalizationsFiles))
                throw new ArgumentNullException(nameof(options.PathToLocalizationsFiles),
                    "Path to localization files is empty");

            CheckLocalizationPath(options.PathToLocalizationsFiles);
        }

        private void CheckLocalizationPath(string optionPath)
        {
            _localizationFilesPath = Path.Combine(_appWorkingDirectory.GetAppDirectory(), optionPath);
            if (!Directory.Exists(_localizationFilesPath))
                throw new ArgumentException($"Localizations directory doesn't exist. Path: {_localizationFilesPath}");
        }

        private string[] GetPathToLocalizationFiles()
        {
            var filePaths = Directory.GetFiles(_localizationFilesPath);
            if (!filePaths.Any())
                throw new Exception("Can't find any localization file");
            return filePaths;
        }
    }

    public class LocalizationLoaderOptions
    {
        public string PathToLocalizationsFiles { get; set; }
    }
}