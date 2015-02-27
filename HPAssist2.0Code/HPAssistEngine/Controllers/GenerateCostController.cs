using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Text;
using HPAssistEngine.Models;
using HPAssistEngine.HPDAL;

namespace HPAssistEngine.Controllers
{
    public class GenerateCostController : ApiController
    {
        [Authorize]
        public HttpResponseMessage post(HttpRequestMessage request)
        {
            HttpResponseMessage response;
            try
            {
                Logging.Info(Convert.ToString(this.GetType()), "GenerateCost Request", "GenerateCost Request Process Started");
                string resultXml = Repository.ProcessHPAssistUploadRequest(request);
                response = Request.CreateResponse(HttpStatusCode.OK, resultXml, "application/xml");
                response.Content = new StringContent(resultXml, Encoding.Unicode);
                Logging.Info(Convert.ToString(this.GetType()), "GenerateCost Request", "GenerateCost Request Process Completed");
                return response;
            }
            catch (Exception ex)
            {
                Logging.Error(Convert.ToString(this.GetType()), "Generate Cost", ex.Message);
                response = Request.CreateResponse(HttpStatusCode.OK, ex.Message, "application/xml");
                response.Content = new StringContent(ex.Message, Encoding.Unicode);
                return response;
            }
        }

    }
}

