using System;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class LongLastingUserActionHandlerFactory : ILongLastingUserActionHandlerFactory
    {
        private readonly ICurrentScopeServiceProvider _serviceProvider;

        public LongLastingUserActionHandlerFactory(ICurrentScopeServiceProvider serviceProvider)
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