using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Core.Interfaces
{
    public interface IRozkladUsersDao
    {
        Task Add(RozkladUser rozkladUser);
        Task Update(RozkladUser rozkladUser);
        Task Delete(RozkladUser rozkladUser);
        Task<RozkladUser> Find(string guid);
        Task<ICollection<RozkladUser>> GetAll();
    }
}