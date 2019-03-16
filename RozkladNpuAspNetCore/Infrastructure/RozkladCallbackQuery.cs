using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RozkladNpuAspNetCore.Infrastructure
{
    public enum RozkladCallbackQueryType
    {
        AddGroup
    }

    public class RozkladCallbackQuery
    {
        public RozkladCallbackQueryType UpdateType;
    }
}
