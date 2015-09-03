using EasyAnalysis.Models;
using EasyAnalysis.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Utility.MSDN;

namespace EasyAnalysis.Controllers
{
    public class ThreadController : ApiController
    {
        private readonly IThreadRepository _threadRepository;

        public ThreadController()
        {
            _threadRepository = new ThreadInMemoryRepository();
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public ThreadModel Get(string id)
        {
            return _threadRepository.Get(id);
        }

        // POST api/values
        public async Task<string> Post([FromBody]string value)
        {
            String identifier = String.Empty;

            Regex urlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var match = urlRegex.Match(value);

            if (!match.Success)
            {
                return identifier;
            }

            identifier = match.Groups[0].ToString();

            if (_threadRepository.Exists(identifier))
            {
                return identifier;
            }

            bool success = await RegisterNewThreadAsync(identifier);

            return success ? identifier : string.Empty;
        }

        private async Task<bool> RegisterNewThreadAsync(string identifier)
        {
            // register a new thread item
            try
            {
                var parser = new ThreadParser(Guid.Parse(identifier));

                var info = await parser.ReadThreadInfoAsync();

                // query the database by the identifer / create a new item if not exist
                var model = new ThreadModel
                {
                    Id = info.Id,
                    Title = info.Title,
                    AuthorId = info.AuthorId,
                    CreateOn = info.CreateOn,
                    ForumId = info.ForumId
                };

                _threadRepository.Create(model);

                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
            // Not Support
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            // Not Support
        }
    }
}
