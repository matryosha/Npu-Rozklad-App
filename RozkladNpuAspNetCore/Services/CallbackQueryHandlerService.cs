using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RozkladNpuAspNetCore.Infrastructure;
using RozkladNpuAspNetCore.Interfaces;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Services
{
    public class CallbackQueryHandlerService : ICallbackQueryHandlerService
    {
        public Task Handle(CallbackQuery callbackQuery)
        {
            if(string.IsNullOrEmpty(callbackQuery.Data)) return Task.CompletedTask;
            var callbackQueryData = CallbackQueryDataConverter.ConvertDataFromString(callbackQuery.Data);
            switch (callbackQueryData.Key)
            {
                case RozkladCallbackQueryType.AddGroup:
                {

                    break;
                }
            }
            return Task.CompletedTask;
        }
    }
}
