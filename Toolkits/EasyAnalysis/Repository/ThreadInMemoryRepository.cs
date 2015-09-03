using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyAnalysis.Models;

namespace EasyAnalysis.Repository
{
    public class ThreadInMemoryRepository : IThreadRepository
    {
        private readonly static IList<ThreadModel> ThreadsStore = new List<ThreadModel>();

        public string Create(ThreadModel model)
        {
            ThreadsStore.Add(model);

            return model.Id;
        }

        public bool Exists(string id)
        {
            return ThreadsStore.FirstOrDefault(m => m.Id.Equals(id)) != null;
        }

        public ThreadModel Get(string id)
        {
            return ThreadsStore.FirstOrDefault(m => m.Id.Equals(id));
        }
    }
}