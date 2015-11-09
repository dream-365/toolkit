using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Analysis
{
    public interface IAction
    {
        string Description { get; }

        Task RunAsync(string[] args);
    }
}
