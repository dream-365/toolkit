using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Cache
{
    public interface ICacheService
    {
        ICacheClient CreateClient();
    }
}
