using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Moonshot.Server.Controllers
{
    public class ThingController : ApiController
    {
        // GET api/thing 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/thing/5 
        public string Get(string id)
        {
            return "value";
        }

        // POST api/thing 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/thing/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/thing/5 
        public void Delete(int id)
        {
        } 
    }
}
