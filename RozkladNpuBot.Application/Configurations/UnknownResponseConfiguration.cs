using System;
using System.Collections.Generic;

namespace RozkladNpuBot.Application.Configurations
{
    public class UnknownResponseConfiguration
    {
        private static readonly Random Random  = new Random();

        public List<string> IdkStickersValues { get; set; }

        public string GetRandomStickerString()
        {
            return IdkStickersValues[Random.Next(0, IdkStickersValues.Count - 1)];
        }
    }
}
