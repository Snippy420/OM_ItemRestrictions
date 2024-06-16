using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OMItemRestrictions.Services
{
    [Service]
    public interface IBlacklistManager
    {
        public void AddBlacklist(string group, int item);
        public Task<string> RemoveBlacklist(string group, int item);
        public void LoadBlacklistToMemory();
        public bool IsItemBlacklisted(int item, out string group);
        public Task<List<string>> BlacklistGroups();
    }
}
