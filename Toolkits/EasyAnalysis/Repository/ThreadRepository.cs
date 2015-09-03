using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyAnalysis.Models;

namespace EasyAnalysis.Repository
{
    public class ThreadRepository : IThreadRepository
    {
        public string Create(ThreadModel model)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string id)
        {
            throw new NotImplementedException();
        }

        public ThreadModel Get(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTagsByThread(string id)
        {
            throw new NotImplementedException();
        }
    }
}