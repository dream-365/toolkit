using EasyAnalysis.Models;
using EasyAnalysis.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EasyAnalysis.Controllers
{
    public class TagController : ApiController
    {
        private readonly ITagRepository _tagRepositry = new TagRepository(new DefaultDbConext());

        // GET: api/Tag
        public IEnumerable<string> Get()
        {
            throw new NotImplementedException();
        }

        // GET: api/Tag/5
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST: api/Tag
        public Tag Post([FromBody]string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return _tagRepositry.CreateTagIfNotExists(value);
        }

        // PUT: api/Tag/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Tag/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
