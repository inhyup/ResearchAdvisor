using Amazon.DynamoDBv2.DataModel;
using ResearchAdvisor.DomainApi.Port;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResearchAdvisor.DomainApi.Model;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using ResearchAdvisor.Persistence.Adapter.Context;

namespace ResearchAdvisor.Domain
{
    public class UserDomain<T> : IRequestUser<T> where T : class
    {
        private readonly IDynamoDBContext _dynamoDBContext;
        private readonly ApplicationDbContext _dbContext;
        //private readonly List<ResearchPaper> _data;

        public UserDomain(IDynamoDBContext dynamoDBContext, ApplicationDbContext dbContext)
        {
            _dynamoDBContext = dynamoDBContext;
            _dbContext = dbContext;
            //_data = _dbContext.Load();
        }

        public async Task<List<T>> GetUserByEmail(string email)
        {
            var result = await _dynamoDBContext.QueryAsync<T>(email).GetRemainingAsync();
            return result;
        }

        public async Task Save(T t)
        {
            await _dynamoDBContext.SaveAsync(t);
        }

        public async Task<T> Load(string email, string password)
        {   
            var user = await _dynamoDBContext.LoadAsync<T>(email, password);
            return user;
        }

        public async Task Delete(T t)
        {
            await _dynamoDBContext.DeleteAsync(t);
        }

        public async Task<Arxiv> SearchAsync(string searchWord, User user)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Arxiv));
            Arxiv arxiv;
            var results = new List<Entry>();
            var res = new Arxiv();
            var keywords = GetSearchKeyword(searchWord);
            var titleKeywords = CombinationKeywords(keywords);

            if (!string.IsNullOrEmpty(user.Email))
            {
                await SaveLikedKeywords(keywords, user);
            }
            var urls = new List<string>();
            //var titleKeywords = GetTitleKeywords(keywords);
            // generate url
            foreach (var title in titleKeywords)
            {
                var url = GenerateURLs(title, keywords, user.LikedCategory);
                urls.AddRange(url);
            }
            urls = urls.OrderByDescending(x => x.Length).ToList();

            if (urls.Count > 7)
            {                
                urls = urls.Take(7).ToList();
            }
            
            int total_results = 200;
            foreach (var url in urls)
            {
                int start = 0;
                int results_per_iteration = 200;
                for (int i = start; i < total_results; i += results_per_iteration)
                {
                    string search_url = $"{url}&start={i}&max_results={results_per_iteration}";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(search_url);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        arxiv = (Arxiv)serializer.Deserialize(reader);
                        results.AddRange(arxiv.Entry);
                    }
                }
                //total_results = (total_results / 2 < 50) ? 50 : total_results / 2;                
            }
            results = results.GroupBy(x => x.id).Select(x => x.First()).ToList();
            res.Entry = results;
            return res;
        }

        public async Task<Arxiv> LoadArxiv(User user)
        {
            var res = new Arxiv();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Arxiv));
                Arxiv arxiv;
                var results = new List<Entry>();
                var keywords = new HashSet<string>();
                var sortKeywords = new Dictionary<string, int>();

                if (string.IsNullOrEmpty(user.Email))
                {
                    return res;
                }
                // get liked papers
                if (user.LikedPapers.Count > 0)
                {
                    Dictionary<string, int> paperKeywordDict = new Dictionary<string, int>();
                    foreach (var paper in user.LikedPapers)
                    {
                        var keys = GetSearchKeyword(paper.title);
                        foreach (var k in keys)
                        {
                            if (paperKeywordDict.ContainsKey(k))
                            {
                                paperKeywordDict[k]++;
                            }
                            else
                            {
                                paperKeywordDict.Add(k, 1);
                            }
                        }
                    }

                    sortKeywords = paperKeywordDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                }
               
                // get liked keywords
                if (user.LikedKeywords.Count != 0)
                {
                    foreach (var k in user.LikedKeywords)
                    {
                        if (sortKeywords.ContainsKey(k.Key))
                        {
                            sortKeywords[k.Key] += k.Value;
                        }
                        else
                        {
                            sortKeywords.Add(k.Key, k.Value);
                        } 
                    }
                    sortKeywords = sortKeywords.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    keywords = sortKeywords.Keys.Take(4).ToHashSet();
                }
                
                var titleKeywords = CombinationKeywords(keywords);
                var urls = new List<string>();

                foreach (var title in titleKeywords)
                {
                    var url = GenerateURLs(title, keywords, user.LikedCategory);
                    urls.AddRange(url);
                }
                urls = urls.OrderByDescending(x => x.Length).ToList();
                if (urls.Count > 7)
                {
                    urls = urls.Take(7).ToList();
                }
                int total_results = 200;
                foreach (var url in urls)
                {
                    int start = 0;
                    int results_per_iteration = 200;
                    for (int i = start; i < total_results; i += results_per_iteration)
                    {
                        string search_url = $"{url}&start={i}&max_results={results_per_iteration}";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(search_url);
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            arxiv = (Arxiv)serializer.Deserialize(reader);
                            results.AddRange(arxiv.Entry);
                        }
                    }
                }
                results = results.GroupBy(x => x.id).Select(x => x.First()).ToList();
                res.Entry = results;
                return res;
            }
            catch
            {
                return res;
            }      
        }

        private async Task SaveLikedKeywords(HashSet<string> keywords, User user)
        {
            foreach (var k in keywords)
            {
                if (user.LikedKeywords.ContainsKey(k))
                {
                    user.LikedKeywords[k] += 1;
                }
                else
                {
                    user.LikedKeywords.Add(k, 1);
                }
            }
            await _dynamoDBContext.SaveAsync(user);
        }

        public async Task<ResearchPapers> SearchPapers(string searchWord, User user)
        {
            var data = _dbContext.Load();
            var lists = new List<List<ResearchPaper>>();
            var result = new ResearchPapers();
            var keywords = GetSearchKeyword(searchWord);
            var titleKeywords = CombinationKeywords(keywords);
            titleKeywords.Reverse();
            if (!string.IsNullOrEmpty(user.Email))
            {
                await SaveLikedKeywords(keywords, user);
            }

            foreach (var d in data)
            {
                var list = new List<ResearchPaper>();
                foreach (var titleKeyword in titleKeywords)
                {
                    if (d.Categories.StartsWith(user.LikedCategory))
                    {
                        if (titleKeyword.Any(d.Title.Contains))
                        {
                            list.Add(d);
                        }
                    }
                }
                lists.Add(list);
            }

            var s = lists.SelectMany(x => x).ToList();
            s = s.GroupBy(x => x.Id).Select(x => x.First()).TakeLast(1000).ToList();
            result.researchPapers = s;
            return result;
        }
        public ResearchPapers LoadPapers(User user)
        {
            var result = new ResearchPapers();
            try
            {
                if (user.Email == null)
                {
                    return result;
                }
                var data = _dbContext.Load();
                var lists = new List<List<ResearchPaper>>();
                var keywords = new HashSet<string>();
                if (user.LikedKeywords.Count != 0)
                {
                    keywords = user.LikedKeywords.Keys.Take(5).ToHashSet();
                }
                var titleKeywords = CombinationKeywords(keywords);
                titleKeywords.Reverse();

                foreach (var d in data)
                {
                    var list = new List<ResearchPaper>();
                    foreach (var titleKeyword in titleKeywords)
                    {
                        if (d.Categories.StartsWith(user.LikedCategory))
                        {
                            if (titleKeyword.Any(d.Title.Contains))
                            {
                                list.Add(d);
                            }
                        }
                    }
                    lists.Add(list);
                }

                if (lists[0].Count == 0)
                {
                    foreach (var d in data)
                    {
                        var list = new List<ResearchPaper>();
                        if (d.Categories.StartsWith(user.LikedCategory))
                        {
                            list.Add(d);
                        }
                        lists.Add(list);
                    }
                }

                var s = lists.SelectMany(x => x).ToList();
                s = s.GroupBy(x => x.Id).Select(x => x.First()).TakeLast(1000).ToList();
                result.researchPapers = s;
                return result;
            }
            catch
            {
                return result;
            }
        }

        private List<HashSet<string>> GetTitleKeywords(HashSet<string> keywords)
        {
            var result = new List<HashSet<string>>();
            for (int i = 0; i < keywords.Count; i++)
            {
                var r = new HashSet<string>();
                for (int j = i; j < keywords.Count; j++)
                {
                    r.Add(keywords.ElementAt(j));
                }
                if (r.Count > 0)
                {
                    result.Add(r);
                }
            }
            return result;
        }

        private List<HashSet<string>> CombinationKeywords(HashSet<string> keywords)
        {
            var result = new List<HashSet<string>>();
            int length = keywords.Count;

            for (int i = 0; i < (1 << length); ++i)
            {
                var combination = new HashSet<string>();
                int count = 0;
                
                for (count = 0; count < length; ++count)
                {
                    if ((i & 1 << count) > 0)
                    {
                        combination.Add(keywords.ElementAt(count)); 
                    }
                }
                if (count > 0 && combination.Count > 0)
                {
                    result.Add(combination);
                }
            }
            return result;
        }

        private HashSet<string> GetSearchKeyword(string searchWord)
        {
            var rake = new Rake.Rake();
            var result = new HashSet<string>();
            var rakeResults = rake.Run(searchWord);
            foreach (var r in rakeResults)
            {
                var tokenWords = TokenizeIntoWords(r.Key);
                foreach (var t in tokenWords)
                {
                    result.Add(t);
                }
            }
            return result;
        }

        private List<string> GenerateURLs(HashSet<string> keywords, HashSet<string> absKeywords, string category)
        {
            var result = new List<string>();

            var sb = new StringBuilder();
            sb.Append("http://export.arxiv.org/api/query?search_query=");
            // add users field
            foreach (var k in keywords)
            {
                sb.Append("ti:");
                sb.Append(k);
                sb.Append("+AND+");
            }

            if (string.IsNullOrEmpty(category))
                {
                    int index = sb.ToString().LastIndexOf("+AND+");
                    sb.Remove(index, 5);
                }
                else
                {
                    sb.Append("cat:");
                    sb.Append(category);
                }
                result.Add(sb.ToString());

            return result;
        }

        private HashSet<string> TokenizeIntoWords(string text)
        {
            var mlContext = new MLContext();
            var result = new HashSet<string>();

            var emptyData = new List<TextData>();
            var emptyDataView = mlContext.Data.LoadFromEnumerable(emptyData);
            var textPipeline = mlContext.Transforms.Text.TokenizeIntoWords("Words", "Text", separators: new[] { ' ' });
            var tokenModel = textPipeline.Fit(emptyDataView);
            var engine = mlContext.Model.CreatePredictionEngine<TextData, TextTokens>(tokenModel);
            var tokens = engine.Predict(new TextData { Text = text });

            foreach (var w in tokens.Words)
            {
                result.Add(w);
            }

            return result;
        }
    }
}
