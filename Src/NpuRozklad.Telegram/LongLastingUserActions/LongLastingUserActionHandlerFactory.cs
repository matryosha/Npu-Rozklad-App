using System;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class LongLastingUserActionHandlerFactory : ILongLastingUserActionHandlerFactory
    {
        private readonly IExternalServiceProvider _serviceProvider;

        public LongLastingUserActionHandlerFactory(IExternalServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public ILongLastingUserActionHandler GetHandler(LongLastingUserActionArguments actionArguments)
        {
            switch (actionArguments.UserActionType)
            {
                case LongLastingUserActionType.TimetableSelectingFaculty:
                    return _serviceProvider.GetService<TimetableSelectingFacultyActionHandler>();
                case LongLastingUserActionType.TimetableSelectingFacultyGroupToAdd:
                    return _serviceProvider.GetService<TimetableSelectingFacultyGroupToAddActionHandler>();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}