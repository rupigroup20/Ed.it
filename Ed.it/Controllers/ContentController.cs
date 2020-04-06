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
using System.Drawing;
using Aspose.Slides;

namespace Ed.it.Controllers
{
    public class ContentController : ApiController
    {
        string UrlServer = "http://proj.ruppin.ac.il/igroup20/prod/uploadFiles/";//ניתוב שרת
        string UrlLocal = @"C:\Users\programmer\ed.it_client\public\uploadedFilesPub\\";//ניתוב מקומי
        string UrlLocalAlmog = @"C:\Users\almog\Desktop\final project development\server\Ed.it\Ed.it\uploadedPictures\\";

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
        [Route("api/Content/AddContent")]
        public HttpResponseMessage AddContent(Content content)
        {
            content.UploadContent();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Error message hadar shame");
            HttpResponseMessage response2 = Request.CreateResponse(HttpStatusCode.Created);

            return response;

        }

        [HttpPost]
        [Route("api/Content/UploadContent/{ByUser}/{ContentID}")]
        public HttpResponseMessage UploadContent(string ByUser,string ContentID)//[FromBody]User NewUser
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

                        //פיצול מצגת לתמונות
                        using (Aspose.Slides.Presentation pres = new Aspose.Slides.Presentation(fileSavePath))
                        {
                            foreach (ISlide sld in pres.Slides)
                            {
                                // Create a full scale image
                                Bitmap bmp = sld.GetThumbnail(1f, 1f);
                                fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedFiles"), string.Format("{0}_{1}.jpg", ContentID, sld.SlideNumber));// $@"{Email.Split('@').First()}_{sld.SlideNumber}"
                                                                                                                                                                                    // Save the image to disk in JPEG format
                                bmp.Save(fileSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }

              
                }
            }
            // Return status code  
            return Request.CreateResponse(HttpStatusCode.Created, imageLinks);
      
        }     

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}