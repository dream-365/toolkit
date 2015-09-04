using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Repository
{
    public interface IThreadRepository
    {
        bool Exists(string id);
        string Create(ThreadModel model);
        ThreadModel Get(string id);
        IEnumerable<string> GetTagsByThread(string id);
        void Change(string id, Action<ThreadModel> model);
    }
}
