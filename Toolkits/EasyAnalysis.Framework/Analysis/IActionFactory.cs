using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Analysis
{
    public interface IActionFactory
    {
        IAction CreateInstance(string name);
    }
}
