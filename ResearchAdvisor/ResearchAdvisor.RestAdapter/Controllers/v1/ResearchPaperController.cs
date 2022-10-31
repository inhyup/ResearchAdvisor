using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using ResearchAdvisor.DomainApi.Model;
using ResearchAdvisor.DomainApi.Port;
using System.Linq;


namespace ResearchAdvisor.RestAdapter.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ResearchPaperController : ControllerBase
    {

        private readonly IRequestResearchPaper<Arxiv> _requestResearch;

        public ResearchPaperController(IRequestResearchPaper<Arxiv> requestResearch)
        {
            _requestResearch = requestResearch;
        }

        // GET: api/research
        [HttpGet]
        public Entry Get(string id)
        {
            var result = _requestResearch.Get(id);            
            return result.Entry.FirstOrDefault();
        }

/*        // GET: api/research/1
        [HttpGet]
        [Route("{id}", Name = "GetResearch")]
        public IActionResult Get(string id)
        {
            var result = _requestResearch.GetResearchPaper(id);
            return Ok(result);
        }*/
    }
}
