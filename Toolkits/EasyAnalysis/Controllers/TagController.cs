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
        public int Post([FromBody]string value)
        {
            throw new NotImplementedException();
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
