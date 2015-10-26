using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Repository
{
    interface ITagRepository
    {
        Tag CreateTagIfNotExists(string name);

        IQueryable<Tag> Search(string q);
    }
}
