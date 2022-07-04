using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ResearchAdvisor.DomainApi.Model;
using ResearchAdvisor.DomainApi.Port;

namespace ResearchAdvisor.RestAdapter.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ResearchController : ControllerBase
    {

        private readonly IRequestResearch<Research> _requestResearch;

        public ResearchController(IRequestResearch<Research> requestResearch)
        {
            _requestResearch = requestResearch;
        }

        // GET: api/research
        [HttpGet]
        public IActionResult Get()
        {
            var result = _requestResearch.GetResearches();
            return Ok(result);
        }

        // GET: api/research/1
        [HttpGet]
        [Route("{id}", Name = "GetResearch")]
        public IActionResult Get(int id)
        {
            var result = _requestResearch.GetResearch(id);
            return Ok(result);
        }


        [HttpPost]
        public void Post(Research research)
        {
            _requestResearch.PostResearch(research);
        }
    }
}
