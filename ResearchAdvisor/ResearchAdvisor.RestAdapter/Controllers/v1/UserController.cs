using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using ResearchAdvisor.DomainApi.Model;
using ResearchAdvisor.DomainApi.Port;
using ResearchAdvisor.Persistence.Adapter.Context;

namespace ResearchAdvisor.RestAdapter.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IRequestUser<User> _requestUser;

        public UserController(IRequestUser<User> requestUser)
        {
            _requestUser = requestUser;
        }

        // GET: api/research/1
        [HttpPost("Login")]
        public async Task<User> Login(User user)
        {
            try
            {
                var result = await _requestUser.Load(user.Email, user.Password);
                return result;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost("Create")]
        public async Task<string> Create(User user)
        {
            try
            {
                if (IsValidEmail(user.Email))
                {
                    var result = await _requestUser.GetUserByEmail(user.Email);
                    if (result.Count == 0)
                    {
                        user.LikedKeywords = new Dictionary<string, int>();
                        user.LikedPapers = new List<Entry>(); 
                        await _requestUser.Save(user);
                        return string.Format("{0} is registered", user.Email);
                    }
                    else
                    {
                        return string.Format("{0} is already existed", user.Email);
                    }
                }
                else
                {
                    return string.Format("Wrong email format {0}", user.Email);
                }
            }
            catch
            {
                return string.Format("Failed to register {0}", user.Email);
            }
        }

        [HttpPut]
        public async Task Update(User user)
        {
            var updatedUser = await _requestUser.Load(user.Email, user.Password);
            updatedUser.Password = user.Password;
            updatedUser.LikedKeywords = user.LikedKeywords;
            updatedUser.LikedCategory = user.LikedCategory;
            updatedUser.LikedPapers = user.LikedPapers;
            await _requestUser.Save(updatedUser);
        }


        [HttpDelete("{email, password}")]
        public async Task Delete(string email, string password)
        {
            var deletedUser = await _requestUser.Load(email, password);
            await _requestUser.Delete(deletedUser);
        }


        [HttpPatch("searchasync/{search_word}")]
        public Task<Arxiv> SearchAsync(string search_word, User user)
        {
            var result = _requestUser.SearchAsync(search_word, user);
            return result;
        }

        [HttpPatch("loadarxiv")]
        public Task<Arxiv> LoadArxiv(User user)
        {
            var result = _requestUser.LoadArxiv(user);
            return result;
        }

        [HttpPatch("loadpapers")]
        public ResearchPapers LoadPapers(User user)
        {
            var result = _requestUser.LoadPapers(user);
            return result;
        }

        [HttpPatch("search/{searchWord}")]
        public Task<ResearchPapers> SearchPapers(string searchWord, User user)
        {
            var result = _requestUser.SearchPapers(searchWord, user);
            return result;
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
