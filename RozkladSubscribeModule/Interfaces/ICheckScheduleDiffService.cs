namespace RozkladSubscribeModuleClient.Interfaces
{
    internal interface ICheckScheduleDiffService<out TCheckPayload>
        where TCheckPayload: ICheckPayload
    {
        TCheckPayload CheckDiff(string facultyShortName, int groupExternalId);
    }
}