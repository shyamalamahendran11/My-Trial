#region Namespace
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
#endregion
namespace HPAssistEngine.Controllers
{
    public class CompareCostController : ApiController
    {
        [Authorize]
        public HttpResponseMessage post(HttpRequestMessage request)
        {
            HttpResponseMessage response;
            try
            {
                Logging.Info(this.GetType().ToString(), "CompareCost Request", "CompareCost Request Process Started");
                string resultXml = Repository.ProcessCompareCostRequest(request);
                response = Request.CreateResponse(HttpStatusCode.OK, resultXml, "application/xml");
                response.Content = new StringContent(resultXml, Encoding.Unicode);
                Logging.Info(this.GetType().ToString(), "CompareCost Request", "CompareCost Request Process Completed");
                return response;
            }
            catch (Exception ex)
            {
                Logging.Error(this.GetType().ToString(), "CompareCost", ex.Message);
                response = Request.CreateResponse(HttpStatusCode.OK, ex.Message, "application/xml");
                response.Content = new StringContent(ex.Message, Encoding.Unicode);
                return response;
            }
        }
    }
}
