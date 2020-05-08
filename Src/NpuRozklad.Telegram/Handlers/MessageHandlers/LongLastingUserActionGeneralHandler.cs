using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NpuRozklad.Telegram.Exceptions;
using NpuRozklad.Telegram.LongLastingUserActions;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.MessageHandlers
{
    public class LongLastingUserActionGeneralHandler
    {
        private readonly ILongLastingUserActionManager _longLastingUserActionManager;
        private readonly ICurrentTelegramUserProvider _currentTelegramUserProvider;
        private readonly ILongLastingUserActionHandlerFactory _longLastingUserActionHandlerFactory;
        private readonly ILogger<LongLastingUserActionGeneralHandler> _logger;

        public LongLastingUserActionGeneralHandler(
            ILongLastingUserActionManager longLastingUserActionManager,
            ICurrentTelegramUserProvider currentTelegramUserProvider,
            ILongLastingUserActionHandlerFactory longLastingUserActionHandlerFactory,
            ILogger<LongLastingUserActionGeneralHandler> logger)
        {
            _longLastingUserActionManager = longLastingUserActionManager;
            _currentTelegramUserProvider = currentTelegramUserProvider;
            _longLastingUserActionHandlerFactory = longLastingUserActionHandlerFactory;
            _logger = logger;
        }

        public async Task<bool> Handle(Message message)
        {
            var currentUser = _currentTelegramUserProvider.GetCurrentTelegramRozkladUser();

            LongLastingUserActionArguments userLongLastingActionArguments = null;
            ILongLastingUserActionHandler handler = null;
            
            try
            {
                bool isHandled = false;
                
                userLongLastingActionArguments = await _longLastingUserActionManager.GetUserLongLastingAction(currentUser);

                if (userLongLastingActionArguments == null) return isHandled;
            
                userLongLastingActionArguments.Parameters[typeof(Message)] = message;
                handler = _longLastingUserActionHandlerFactory.GetHandler(userLongLastingActionArguments);
                isHandled = await handler.Handle(userLongLastingActionArguments);
            
                return isHandled;
            }
            catch (Exception e)
            {
                await _longLastingUserActionManager.ClearUserAction(currentUser);

                var currentUserTelegramId = currentUser.TelegramId;
                var outException = new LongLastingUserActionHandlerException(currentUserTelegramId, e);

                var messageId = message.MessageId;
                var messageFromUser = message.From;
                var messageText = message.Text;

                _logger.LogError(TelegramLogEvents.LongLastingUserActionError, outException,
                    "Current user telegram id: {currentUserTelegramId}. " +
                    "userLongLastingActionArguments: {userLongLastingActionArguments}. " +
                    "handler: {handler}. " +
                    "Telegram message id : {messageId}. " +
                    "Message from user: {messageFromUser}. " +
                    "Message text: {messageText}",
                    currentUserTelegramId, userLongLastingActionArguments, handler, messageId, messageFromUser,
                    messageText);
                
                return false;
            }
        }
    }
}