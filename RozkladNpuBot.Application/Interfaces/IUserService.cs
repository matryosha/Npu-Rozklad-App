using System.Threading.Tasks;
using RozkladNpuBot.Domain.Entities;

namespace RozkladNpuBot.Application.Interfaces
{
    public interface IUserService
    {
        Task AddUser(RozkladUser user);

        Task UpdateUser(RozkladUser user);

        Task<RozkladUser> GetUser(int telegramId);

        /// <summary>
        /// Last saved message id sent by bot.
        /// Uses for editing message
        /// </summary>
        bool TryGetLastMessageId(long chatId, out int messageId);

        void SetLastMessageId(long chatId, int messageId);

    }
}
