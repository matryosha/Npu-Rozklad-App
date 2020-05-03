using System;
using NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers
{
    internal class SpecificCallbackQueryHandlerProvider
    {
        private readonly IExternalServiceProvider _serviceProvider;

        public SpecificCallbackQueryHandlerProvider(IExternalServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        internal ISpecificCallbackQueryHandler GetHandler(CallbackQueryActionType actionType)
        {
            switch (actionType)
            {
                case CallbackQueryActionType.AddGroup:
                    return _serviceProvider.GetService<AddGroupCallbackHandler>();
                case CallbackQueryActionType.ShowTimetableFacultyGroupViewMenu:
                    return _serviceProvider.GetService<ShowTimetableFacultyGroupViewMenuCallbackHandler>();
                case CallbackQueryActionType.ShowTimetableFacultyGroupsMenu:
                    return _serviceProvider.GetService<ShowTimetableFacultyGroupsMenuCallbackHandler>();
                case CallbackQueryActionType.TimetableFacultyGroupsMenuGroupSelected:
                    return _serviceProvider.GetService<TimetableFacultyGroupsMenuGroupSelectedHandler>();
                case CallbackQueryActionType.ShowScheduleMenu:
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