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

        public void Change(string id, Action<ThreadModel> applyModelChange)
        {
            var modelToChange = FindByIdIncludeTags(id);

            applyModelChange(modelToChange);

            _context.SaveChanges();
        }

        public string Create(ThreadModel model)
        {
            var item = _context.Threads.Add(model);

            _context.SaveChanges();

            return item.Id;
        }

        public bool Exists(string id)
        {
            var result = _context.Threads.Find(id);

            return result != null;
        }

        public ThreadModel Get(string id)
        {
            return FindByIdIncludeTags(id);
        }

        private ThreadModel FindByIdIncludeTags(string id)
        {
            return _context.Threads
                .Include("Tags")
                .FirstOrDefault(m => m.Id.Equals(id));
        }

        public IEnumerable<string> GetTagsByThread(string id)
        {
            throw new NotImplementedException();
        }
    }
}