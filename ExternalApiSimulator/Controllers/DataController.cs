using Bogus;
using ExternalApiSimulator.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalApiSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        //private const int TOTAL_RECORDS = 1_000_000; // simulate 1 million
        private const int TOTAL_RECORDS = 20000; // simulate 1 million

        [HttpGet]
        public IActionResult Get(int page = 1, int pageSize = 1000)
        {
            var skip = (page - 1) * pageSize;

            if (skip >= TOTAL_RECORDS)
                return Ok(new List<ExternalRecordDto>());

            var faker = new Faker<ExternalRecordDto>()
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Email, f => f.Internet.Email());

            var data = faker.Generate(pageSize);

            return Ok(data);
        }
    }
}
