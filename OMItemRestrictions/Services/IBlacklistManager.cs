using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace OMItemRestrictions.Services
{
    [Service]
    public interface IBlacklistManager
    {
        public UniTask AddBlacklist(string group, int item);
        public UniTask RemoveBlacklist(string group, int item);
        public UniTask LoadBlacklistToMemory();
        public bool IsItemBlacklisted(int item, out string group);
        public UniTask<List<string>> BlacklistGroups();
    }
}
