using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace OMItemRestrictions.Services
{
    [Service]
    public interface IBlacklistManager
    {
        public void AddBlacklist(string group, int item);
        public void RemoveBlacklist(string group, int item);
        public void LoadBlacklistToMemory();
        public bool IsItemBlacklisted(int item, out string group);
    }
}
