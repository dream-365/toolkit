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
        private readonly ITagRepository _tagRepository;

        public ThreadController()
        {
            var context = new DefaultDbConext();
            _threadRepository = new ThreadRepository(context);
            _tagRepository = new TagRepository(context);
        }

        // GET api/thread
        public IEnumerable<string> Get()
        {
            throw new NotImplementedException();
        }

        // GET api/values/5
        public ThreadModel Get(string id)
        {
            return _threadRepository.Get(id);
        }


        // TODO: CODE_REFACTOR
        [Route("api/thread/{repository}/types"), HttpGet]
        public CategoryResult GetTypes(string repository)
        {
            var provider = new TypeProvider();

            var types = provider.GetTypesByRepository(repository);

            var result = new CategoryResult();

            var categories = types.Select(m => m.CategoryName).Distinct().ToList();

            var typeGroups = new List<IEnumerable<TypeObject>>();

            var tempCategories = new List<Category>();

            int i = 0;

            categories.ForEach((cat) =>
            {
                tempCategories.Add(new Category { index = i++, name = cat });
            });

            result.typeGroups = typeGroups;
            result.categories = tempCategories;

            foreach (var category in result.categories)
            {
                var group = types
                    .Where(m => m.CategoryName == category.name)
                    .Select(m => new TypeObject { id = m.Id, name = m.TypeName });

                typeGroups.Add(group);
            }

            return result;
        }

        [Route("api/thread/{id}/detail"), HttpGet]
        public ThreadViewModel GetDetail(string id)
        {
            var basic = _threadRepository.Get(id);

            var vm = new ThreadViewModel
            {
                Id = basic.Id,
                Title = basic.Title,
                TypeId = basic.TypeId.GetValueOrDefault(),
                Tags = basic.Tags
                            .Select(m => m.Name)
                            .AsEnumerable()
            };

            return vm;
        }

        [Route("api/thread/{id}/classify/{typeId}"), HttpPost]
        public void Classify(string id, int typeId)
        {
            _threadRepository.Change(id, (model) => {
                if (model != null)
                {
                    model.TypeId = typeId;
                }
            });
        }

        // POST api/values
        public async Task<string> Post([FromBody]string value)
        {
            bool success = false;

            string identifier = string.Empty;

            Regex urlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var match = urlRegex.Match(value);

            if (!match.Success)
            {
                return null;
            }

            identifier = match.Groups[0].ToString();

            if (!_threadRepository.Exists(identifier))
            {
                success = await RegisterNewThreadAsync(identifier);
            }
            else
            {
                success = true;
            }

            // HARD CODE, NEED TO REFACTOR
            return success
                ? identifier
                : null;
        }

        [Route("api/thread/{id}/tag"), HttpPost]
        public string AddTagToThread(string id, [FromBody]string value)
        {
            _threadRepository.Change(id, (model) => {
                if (model != null && !model.Tags
                         .Select(m => m.Name.ToLower())
                         .Contains(value.ToLower()))
                {
                    var newTag = _tagRepository.CreateTagIfNotExists(value);

                    model.Tags.Add(newTag);
                }
            });

            return value;
        }

        private async Task<bool> RegisterNewThreadAsync(string identifier)
        {
            // register a new thread item
            try
            {
                var parser = new ThreadParser(Guid.Parse(identifier));

                var info = await parser.ReadThreadInfoAsync();

                if(info == null)
                {
                    return false;
                }

                // query the database by the identifer / create a new item if not exist
                var model = new ThreadModel
                {
                    Id = info.Id,
                    Title = info.Title,
                    AuthorId = info.AuthorId,
                    CreateOn = info.CreateOn,
                    ForumId = info.ForumId
                };

                var tags = Utils.DetectTagsFromTitle(info.Title);

                foreach (var name in tags)
                {
                    var tag = _tagRepository.CreateTagIfNotExists(name);

                    model.Tags.Add(tag);
                }

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
