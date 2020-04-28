using System;
using NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers
{
    internal class SpecificCallbackQueryHandlerProvider
    {
        private readonly IExternalServiceFactory _serviceFactory;

        public SpecificCallbackQueryHandlerProvider(IExternalServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        
        internal ISpecificCallbackQueryHandler GetHandler(CallbackQueryActionType actionType)
        {
            switch (actionType)
            {
                case CallbackQueryActionType.AddGroup:
                    return _serviceFactory.GetService<AddGroupCallbackHandler>();
                case CallbackQueryActionType.ShowTimetableFacultyGroupViewMenu:
                    return _serviceFactory.GetService<ShowTimetableFacultyGroupViewMenuCallbackHandler>();
                case CallbackQueryActionType.ShowTimetableFacultyGroupsMenu:
                    return _serviceFactory.GetService<ShowTimetableFacultyGroupsMenuCallbackHandler>();
                case CallbackQueryActionType.ShowScheduleMenu:
                    break;
                case CallbackQueryActionType.DeleteGroup:
                    break;
                case CallbackQueryActionType.ShowNotificationMenuForGroup:
                    break;
                case CallbackQueryActionType.SubscribeToScheduleNotification:
                    break;
                case CallbackQueryActionType.UnsubscribeFromScheduleNotification:
                    break;
                case CallbackQueryActionType.ShowNotificationsMenu:
                    break;
            }

            throw new NotImplementedException();
        }
    }
}