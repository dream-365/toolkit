using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Repository
{
    public interface ITypeProvider
    {
        IEnumerable<TypeModel> GetTypesByRepository(string repository);
    }
}
