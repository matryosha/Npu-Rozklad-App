using System.Threading.Tasks;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public interface ILongLastingUserActionHandler
    {
        Task<bool> Handle(LongLastingUserActionArguments userActionArguments);
    }
}