using System;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class LongLastingUserActionHandlerFactory : ILongLastingUserActionHandlerFactory
    {
        private readonly IExternalServiceFactory _serviceFactory;

        public LongLastingUserActionHandlerFactory(IExternalServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        
        public ILongLastingUserActionHandler GetHandler(LongLastingUserActionArguments actionArguments)
        {
            switch (actionArguments.UserActionType)
            {
                case LongLastingUserActionType.TimetableSelectingFaculty:
                    return _serviceFactory.GetService<TimetableSelectingFacultyActionHandler>();
                case LongLastingUserActionType.TimetableSelectingFacultyGroupToAdd:
                    return _serviceFactory.GetService<TimetableSelectingFacultyGroupToAddActionHandler>();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}