namespace KMBProofOfConcept.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return value;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"return value is: '{id}'";
        }
    }
}