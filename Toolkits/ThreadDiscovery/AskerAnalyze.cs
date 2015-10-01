using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.MSDN;

namespace ThreadDiscovery
{
    public class UserActivity
    {
        public UserActivity ()
        {
            QuestionCount = 0;
        }

        public UserInfo User { get; set; }

        public int QuestionCount { get; set; }
    }

    public class AskerAnalyzeResult
    {
        public IList<UserActivity> UserActivities { get; set; }

        public AskerAnalyzeResult()
        {
            UserActivities = new List<UserActivity>();
        }

        public override string ToString()
        {
            var list = UserActivities.OrderByDescending(m => m.QuestionCount);

            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }
    }

    public class AskerAnalyze : IThreadAnalyze
    {
        private const string RESULT_KEY = "asker_analyze_result";

        public void Analyze(IDictionary<string, object> result, ThreadInfo info)
        {
            AskerAnalyzeResult analyzeResult = EnsureTheResult(result);

            var activity = analyzeResult.UserActivities.FirstOrDefault(m => m.User.Id.Equals(info.AuthorId));

            if (activity != null)
            {
                activity.QuestionCount = activity.QuestionCount + 1;
            }
            else
            {
                analyzeResult.UserActivities.Add(new UserActivity { User = info.Author, QuestionCount = 1 });
            }
        }

        private static AskerAnalyzeResult EnsureTheResult(IDictionary<string, object> result)
        {
            if (!result.ContainsKey(RESULT_KEY))
            {
                result[RESULT_KEY] = new AskerAnalyzeResult();
            }

            AskerAnalyzeResult analyzeResult = result[RESULT_KEY] as AskerAnalyzeResult;

            if (analyzeResult == null)
            {
                throw new Exception("the analyze result is not matched!");
            }

            return analyzeResult;
        }
    }
}
