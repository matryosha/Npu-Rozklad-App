using System.Threading.Tasks;
using NpuRozklad.Telegram.LongLastingUserActions;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.MessageHandlers
{
    public class LongLastingUserActionGeneralHandler
    {
        private readonly ILongLastingUserActionManager _longLastingUserActionManager;
        private readonly ICurrentTelegramUserContext _currentUserContext;
        private readonly ILongLastingUserActionHandlerFactory _longLastingUserActionHandlerFactory;

        public LongLastingUserActionGeneralHandler(
            ILongLastingUserActionManager longLastingUserActionManager,
            ICurrentTelegramUserContext currentUserContext,
            ILongLastingUserActionHandlerFactory longLastingUserActionHandlerFactory)
        {
            _longLastingUserActionManager = longLastingUserActionManager;
            _currentUserContext = currentUserContext;
            _longLastingUserActionHandlerFactory = longLastingUserActionHandlerFactory;
        }

        public async Task<bool> Handle(Message message)
        {
            bool isHandled = false;
            var userLongLastingActionArguments =
                await _longLastingUserActionManager.GetUserLongLastingAction(_currentUserContext.TelegramRozkladUser);

            if (userLongLastingActionArguments == null) return isHandled;
            
            userLongLastingActionArguments.Parameters[typeof(Message)] = message;
            var handler = _longLastingUserActionHandlerFactory.GetHandler(userLongLastingActionArguments);
            isHandled = await handler.Handle(userLongLastingActionArguments);
            
            return isHandled;
        }
    }
}