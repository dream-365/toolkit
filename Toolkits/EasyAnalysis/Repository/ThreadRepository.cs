using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyAnalysis.Models;

namespace EasyAnalysis.Repository
{
    public class ThreadRepository : IThreadRepository
    {
        private readonly DefaultDbConext _context = new DefaultDbConext();

        public string Create(ThreadModel model)
        {
            var item = _context.Threads.Add(model);

            _context.SaveChanges();

            return item.Id;
        }

        public bool Exists(string id)
        {
            var result = _context.Threads.FirstOrDefault(m => m.Id.Equals(id));

            return result != null;
        }

        public ThreadModel Get(string id)
        {
            return _context.Threads.FirstOrDefault(m => m.Id.Equals(id));
        }

        public IEnumerable<string> GetTagsByThread(string id)
        {
            throw new NotImplementedException();
        }
    }
}