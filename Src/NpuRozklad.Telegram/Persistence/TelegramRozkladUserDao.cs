using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Telegram.Persistence
{
    internal class TelegramRozkladUserDao : ITelegramRozkladUserDao
    {
        private readonly TelegramDbContext _dbContext;
        private readonly IRozkladUsersDao _rozkladUsersDao;

        public TelegramRozkladUserDao(TelegramDbContext dbContext,
            IRozkladUsersDao rozkladUsersDao)
        {
            _dbContext = dbContext;
            _rozkladUsersDao = rozkladUsersDao;
        }
        
        public async Task<TelegramRozkladUser> FindByTelegramId(int telegramId)
        {
            var telegramRozkladUser =
                await _dbContext.TelegramRozkladUsers
                    .AsNoTracking()
                    .Where(r => r.TelegramId == telegramId)
                    .FirstOrDefaultAsync();

            if (telegramRozkladUser == null) return null;

            var rozkladUser = await _rozkladUsersDao.Find(telegramRozkladUser.Guid);

            return rozkladUser == null ? null : telegramRozkladUser.FillFromRozkladUser(rozkladUser);
        }

        public async Task Add(TelegramRozkladUser telegramRozkladUser)
        {
            var alreadyExistedUser =
                await _dbContext.TelegramRozkladUsers
                    .AsNoTracking()
                    .Where(u => u.TelegramId == telegramRozkladUser.TelegramId)
                    .FirstOrDefaultAsync();

            if (alreadyExistedUser == null)
            {
                await _dbContext.TelegramRozkladUsers.AddAsync(telegramRozkladUser);
                await _rozkladUsersDao.Add(telegramRozkladUser);
            }
            else
            {
                _dbContext.TelegramRozkladUsers.Update(telegramRozkladUser);
                await _rozkladUsersDao.Update(telegramRozkladUser);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(TelegramRozkladUser telegramRozkladUser)
        {
            await _rozkladUsersDao.Delete(telegramRozkladUser);
            _dbContext.TelegramRozkladUsers.Update(telegramRozkladUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(TelegramRozkladUser telegramRozkladUser)
        {
            _dbContext.Update(telegramRozkladUser);
            await _rozkladUsersDao.Update(telegramRozkladUser);
            await _dbContext.SaveChangesAsync();
        }
    }
}