using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public interface IPathResolver
    {
        string Resolve(Uri contentUri);
    }
}
