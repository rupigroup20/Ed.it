using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ed.it.Models;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Web.Hosting;

namespace Ed.it.Controllers
{
    public class ContentController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("api/UploadContent/{UserID}")]
        public HttpResponseMessage UploadContent()
        {
            List<string> imageLinks = new List<string>();
            var httpContext = HttpContext.Current;

            // Check for any uploaded file  
            if (httpContext.Request.Files.Count > 0)
            {
                //Loop through uploaded files  
                for (int i = 0; i < httpContext.Request.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];

                    // this is an example of how you can extract addional values from the Ajax call
                    string name = httpContext.Request.Form["user"];

                    if (httpPostedFile != null)
                    {
                        // Construct file save path  
                        //var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);
                        string fname = httpPostedFile.FileName.Split('\\').Last();
                        var fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedFiles"), fname);
                        // Save the uploaded file  
                        httpPostedFile.SaveAs(fileSavePath);
                        imageLinks.Add("uploadedFiles/" + fname);
                    }
                }
            }

            // Return status code  
            return Request.CreateResponse(HttpStatusCode.Created, imageLinks);
            ////העלאת קובץ לשרת
            //var ctx = HttpContext.Current;
            //var root = ctx.Server.MapPath("~/App_Data");//להוסיף תיקייה כזאת בהמשך
            //var provider = new MultipartFormDataStreamProvider(root);
            //try
            //{
            //    await Request.Content.ReadAsMultipartAsync(provider);
            //    foreach (var file in provider.FileData)
            //    {
            //        var name = file.Headers.ContentDisposition.FileName;
            //        var localFileName = file.LocalFileName;
            //        var filePath = Path.Combine(root, name);
            //        File.Move(localFileName, filePath);

            //    }
            //    //content.UploadContent("");
            //    return "File Uploaded";

            //}
            //catch (Exception ex)
            //{
            //    throw (ex.InnerException);
            //}
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}