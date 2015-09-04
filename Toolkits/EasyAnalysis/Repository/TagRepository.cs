using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly DefaultDbConext _context;

        public TagRepository(DefaultDbConext context)
        {
            _context = context;
        }

        public Tag CreateTagIfNotExists(string name)
        {
            var findByName = _context.Tags
                .FirstOrDefault(m => m.Name.Equals(name));

            if(findByName == null)
            {
                findByName = _context.Tags.Add(new Models.Tag {
                    Name = name
                });

                _context.SaveChanges();
            }

            return findByName;
        }
    }
}