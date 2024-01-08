using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beymen.Core.Caching
{
    public interface ICacheManager
    {
        Task<string> GetAsync(string key);

        Task<string> UpdateAsync(string key, string value, TimeSpan? expiry = null);

        Task DeleteAsync(string key);
    }
}
