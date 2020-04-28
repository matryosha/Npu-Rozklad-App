namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public interface ILongLastingUserActionHandlerFactory
    {
        ILongLastingUserActionHandler GetHandler(LongLastingUserActionArguments actionArguments);
    }
}