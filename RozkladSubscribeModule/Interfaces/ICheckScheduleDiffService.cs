using System.Threading.Tasks;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ICheckScheduleDiffService<TCheckPayload>
        where TCheckPayload: ICheckPayload
    {
        Task<TCheckPayload> CheckDiff(string facultyShortName, int groupExternalId);
    }
}