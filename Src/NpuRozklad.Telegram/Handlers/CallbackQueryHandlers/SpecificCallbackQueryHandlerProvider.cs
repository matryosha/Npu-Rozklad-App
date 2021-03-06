using System;
using NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers
{
    internal class SpecificCallbackQueryHandlerProvider
    {
        private readonly ICurrentScopeServiceProvider _serviceProvider;

        public SpecificCallbackQueryHandlerProvider(ICurrentScopeServiceProvider serviceProvider)
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
                case CallbackQueryActionType.OpenRemoveGroupsMenu:
                    return _serviceProvider.GetService<OpenRemoveGroupsMenuCallbackHandler>();
                case CallbackQueryActionType.RemoveGroup:
                    return _serviceProvider.GetService<RemoveGroupCallbackHandler>();
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