
namespace RozkladSubscribeModule.Interfaces
{
    internal interface ICheckToNotifyPayloadConverter<in TCheckPayload, out TNotifyPayload> 
        where TCheckPayload: ICheckPayload 
        where TNotifyPayload: INotifyPayload
    {
        TNotifyPayload Convert(TCheckPayload checkPayload);
    }
}