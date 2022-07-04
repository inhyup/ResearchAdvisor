using Microsoft.AspNetCore.Mvc;
using ResearchAdvisor.DomainApi.Model;
using ResearchAdvisor.DomainApi.Port;

namespace ResearchAdvisor.RestAdapter.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DealController : ControllerBase
    {
        private readonly IRequestDeal<Deal> _requestDeal;

        public DealController(IRequestDeal<Deal> requestDeal)
        {
            _requestDeal = requestDeal;
        }

        // GET: api/deal
        [HttpGet]
        public IActionResult Get()
        {
            var result = _requestDeal.GetDeals();
            return Ok(result);
        }

        // GET: api/deal/1
        [HttpGet]
        [Route("{id}", Name = "GetDeal")]
        public IActionResult Get(int id)
        {
            var result = _requestDeal.GetDeal(id);
            return Ok(result);
        }
    }
}
