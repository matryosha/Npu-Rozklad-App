using System;
using System.Collections.Generic;

namespace RozkladNpuAspNetCore.Configurations
{
    public class UnknownResponseConfiguration
    {
        private static readonly Random Random  = new Random();

        public List<string> StickersString { get; set; }

        public string GetRandomStickerString()
        {
            return StickersString[Random.Next(0, StickersString.Count - 1)];
        }
    }
}
