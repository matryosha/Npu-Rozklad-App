using System;
using System.Collections.Generic;

namespace RozkladNpuAspNetCore.Utils
{
    public class IdkStickers
    {
        private Random _random;
        public List<string> IdkStickersValues { get; set; }

        public string GetRandomStickerString()
        {
            _random = new Random();
            return IdkStickersValues[_random.Next(0, IdkStickersValues.Count - 1)];
        }
    }
}
