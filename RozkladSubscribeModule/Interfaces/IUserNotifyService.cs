namespace RozkladSubscribeModuleClient.Interfaces
{
    internal interface IUserNotifyService<in TNotifyPayload>
        where TNotifyPayload: INotifyPayload
    {
        void Notify(TNotifyPayload payload);
    }
}