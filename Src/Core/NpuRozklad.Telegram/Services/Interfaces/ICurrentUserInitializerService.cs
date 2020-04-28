using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ICurrentUserInitializerService
    {
        Task InitializeCurrentUser(Update update);
    }
}