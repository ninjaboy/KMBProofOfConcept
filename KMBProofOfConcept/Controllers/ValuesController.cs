using Microsoft.AspNetCore.Mvc;

namespace KMBProofOfConcept.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpPost]
        public string Post([FromBody]string value)
        {
            return $"You posted: '{value}'";
        }
    }
}
