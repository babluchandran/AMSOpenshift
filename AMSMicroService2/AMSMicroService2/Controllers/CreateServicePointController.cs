using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMSMicroService2.DatabaseOps;
using AMSMicroService2.Models;
using AMSMicroService2.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMSMicroService2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CreateServicePointController : ControllerBase
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        public IActionResult MicroServiceServicePointGeneration(OPLD opldObject)
        {
            try
            {
                log.Info(DateTime.Now.ToString() + " AMS-POC-MicroServiceServicePointGeneration: Read OPLD message and Start processing.");
                //CreateServicepoint                          
                SakilaContext context = new SakilaContext("server=127.0.01;port=3306;database=ams;user=root;password=techM@Ups1");//HttpContext.RequestServices.GetService(typeof(SakilaContext)) as SakilaContext;

                log.Info(DateTime.Now.ToString() + " AMS-POC-MicroServiceServicePointGeneration: Getting OPLD tracking number matching DIALS record.");
                //Check if opld tracking number matches with dials matching number
                DIALS dialsObject = context.GetMatchingDialsID(opldObject.TrackingNumber);

                ServicePoint servicePointObject = new ServicePoint();
                if (!string.IsNullOrEmpty(dialsObject.TrackingNumber))
                {
                    //Match found 
                    //Compare OPLD Add1 with SP Add1 and Update timestamp and increase Stop ID
                    ServicePoint servicePoint =  context.CompareOPLDAdd1WithSPAddr1(opldObject.AddressLine1);
                    if (servicePoint != null && (servicePoint.ServicePointID != 0))
                    {
                        context.UpdateServicePoint(servicePoint.ServicePointID);
                    }
                    else
                    {
                        servicePointObject = ServicePointUtility.CreateServicePoint(opldObject, dialsObject.ConsigneeName, dialsObject.ClarifiedSignature, true);
                        log.Info(DateTime.Now.ToString() + " AMS-POC-MicroServiceServicePointGeneration: Writing ServicePoint in to DB.");

                        //Write Servicepoint to DB
                        context.AddNewServicePoint(servicePointObject);
                    }
                }
                else
                {
                    //Create Service Point with Stop ID = 1
                    servicePointObject = ServicePointUtility.CreateServicePoint(opldObject, opldObject.AttentionName, "", false);
                    log.Info(DateTime.Now.ToString() + " AMS-POC-MicroServiceServicePointGeneration: Writing ServicePoint in to DB.");

                    //Write Servicepoint to DB
                    context.AddNewServicePoint(servicePointObject);
                }

                //Update OPLD Processing Status
                context.UpdateOPLDProcessingStatus(opldObject.TrackingNumber);
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now.ToString() + " AMS-POC-MicroServiceServicePointGeneration: " + Convert.ToString(ex.Message));
                return new JsonResult(new { Result = System.Net.HttpStatusCode.InternalServerError });
            }

            return Ok(new { Result = "Success" });
        }

        public IActionResult TestMethod()
        {
            return Ok();
        }
    }
}