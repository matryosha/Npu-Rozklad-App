using System;
using System.Collections.Generic;

namespace RozkladNpuAspNetCore.Configurations
{
    public class UnknownResponseConfiguration
    {
        private Random _random;
        public List<string> StickersString { get; set; }

        public string GetRandomStickerString()
        {
            _random = new Random();
            return StickersString[_random.Next(0, StickersString.Count - 1)];
        }
    }
}
